provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
locals {
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-staging-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/uh-resident-information-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "staging_vpc" {
  tags = {
    Name = "vpc-staging-apis-staging"
  }
}
data "aws_subnet_ids" "staging" {
  vpc_id = data.aws_vpc.staging_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

data "aws_ssm_parameter" "uh_postgres_db_password" {
   name = "/uh-api/staging/postgres-password"
 }

 data "aws_ssm_parameter" "uh_postgres_username" {
   name = "/uh-api/staging/postgres-username"
 }

 module "postgres_db_staging" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = "staging"
  vpc_id = data.aws_vpc.staging_vpc.id
  db_identifier = "uh-mirror"
  db_name = "uh_mirror"
  db_port  = 5302
  subnet_ids = data.aws_subnet_ids.staging.ids
  db_engine = "postgres"
  db_engine_version = "11.1" //DMS does not work well with v12
  db_instance_class = "db.t2.micro"
  db_allocated_storage = 20
  maintenance_window ="sun:10:00-sun:10:30"
  db_username = data.aws_ssm_parameter.uh_postgres_username.value
  db_password = data.aws_ssm_parameter.uh_postgres_db_password.value
  storage_encrypted = false
  multi_az = false //only true if production deployment
  publicly_accessible = false
  project_name = "platform apis"
}

/*    DMS SET UP    */

 data "aws_ssm_parameter" "uh_postgres_hostname" {
   name = "/uh-api/staging/postgres-hostname"
}

data "aws_ssm_parameter" "uh_test_hostname" {
    name = "/uh-api/test-server/hostname"
}

/* ONE OFF ACCOUNT SET UP FOR DMS REQUIRED ROLES */

 data "aws_iam_policy_document" "dms_assume_role" {
   statement {
     actions = ["sts:AssumeRole"]

     principals {
       identifiers = ["dms.amazonaws.com"]
       type        = "Service"
     }
   }
 }

/* UHT Credentials*/

data "aws_ssm_parameter" "uht_test_username" {
    name = "/uh-api/test-server/uht-username"
}
data "aws_ssm_parameter" "uht_test_password" {
    name = "/uh-api/test-server/uht-password"
}
/* target enpoint */
module "target_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = "uh_mirror"
    dms_endpoint_identifier = "target-uh-test-endpoint"
    engine_name = "postgres"
    database_port = 5302
    db_username = data.aws_ssm_parameter.uh_postgres_username.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_password = data.aws_ssm_parameter.uh_postgres_db_password.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_server = data.aws_ssm_parameter.uh_postgres_hostname.value
    ssl_mode = "none"
    endpoint_type = "target"
    environment_name = "staging"
    project_name = "resident-information-api"
}

/* UHT source enpoint */
module "uht_source_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = "uhwtest"
    dms_endpoint_identifier = "source-uh-uht-test-endpoint"
    engine_name = "sqlserver"
    database_port = 1433
    db_username = data.aws_ssm_parameter.uht_test_username.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_password = data.aws_ssm_parameter.uht_test_password.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_server = data.aws_ssm_parameter.uh_test_hostname.value
    ssl_mode = "none"
    endpoint_type = "source"
    environment_name = "staging"
    project_name = "resident-information-api"
}

/* UHT DMS replication task */
module "dms_uht_setup_staging" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_replication_task"
    environment_name = "staging" //used for resource tags
    project_name = "resident-information-api" //used for resource tags
    //dms task set up
    replication_instance_arn = "arn:aws:dms:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:rep:DNTOW6TGQEGCAOWQMZYHQRTWAA"
    migration_type = "full-load-and-cdc"
    replication_task_indentifier = "uh-uht-api-dms-task"
    task_settings = file("${path.module}/task_settings.json")
    task_table_mappings = file("${path.module}/uhw_selection_rules.json")
    //replication endpoints
    source_endpoint_arn = module.uht_source_dms_endpoint.dms_endpoint_arn
    target_endpoint_arn = module.target_dms_endpoint.dms_endpoint_arn
}

/* UHW Credentials*/

data "aws_ssm_parameter" "uhw_test_username" {
    name = "/uh-api/test-server/uht-username"
}
data "aws_ssm_parameter" "uhw_test_password" {
    name = "/uh-api/test-server/uhw-password"
}
/* UHW source endpoint */
module "uhw_source_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = "uhwtest"
    dms_endpoint_identifier = "source-uh-uhw-test-endpoint"
    engine_name = "sqlserver"
    database_port = 1433
    db_username = data.aws_ssm_parameter.uhw_test_username.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_password = data.aws_ssm_parameter.uhw_test_password.value //ensure you save your on-prem credentials to the Parameter store and reference it here
    db_server = data.aws_ssm_parameter.uh_test_hostname.value
    ssl_mode = "none"
    endpoint_type = "source"
    environment_name = "staging"
    project_name = "resident-information-api"
}
/* UHW DMS replication task */
module "dms_uhw_setup_staging" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_replication_task"
    environment_name = "staging" //used for resource tags
    project_name = "resident-information-api" //used for resource tags
    //dms task set up
    replication_instance_arn = "arn:aws:dms:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:rep:DNTOW6TGQEGCAOWQMZYHQRTWAA"
    migration_type = "full-load-and-cdc"
    replication_task_indentifier = "uh-uhw-api-dms-task"
    task_settings = file("${path.module}/task_settings.json")
    task_table_mappings = file("${path.module}/uhw_selection_rules.json")
    //replication endpoints
    source_endpoint_arn = module.uhw_source_dms_endpoint.dms_endpoint_arn
    target_endpoint_arn = module.target_dms_endpoint.dms_endpoint_arn
}

