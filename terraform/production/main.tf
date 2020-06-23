# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

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
    bucket  = "terraform-state-production-apis"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/uh-resident-information-api/state"
  }
}

/*    POSTGRES SET UP    */
data "aws_vpc" "production_vpc" {
  tags = {
    Name = "vpc-production-apis-production"
  }
}
data "aws_subnet_ids" "production" {
  vpc_id = data.aws_vpc.production_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}
data "aws_ssm_parameter" "uh_postgres_db_password" {
   name = "/uh-api/production/postgres-password"
}
data "aws_ssm_parameter" "uh_postgres_username" {
   name = "/uh-api/production/postgres-username"
}

module "postgres_db_production" {
  source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
  environment_name = "production"
  vpc_id = data.aws_vpc.production_vpc.id
  db_identifier = "uh-mirror"
  db_name = "uh_mirror"
  db_port  = 5303
  subnet_ids = data.aws_subnet_ids.production.ids
  db_engine = "postgres"
  db_engine_version = "11.1" //DMS does not work well with v12
  db_instance_class = "db.t2.micro"
  db_allocated_storage = 20
  maintenance_window ="sun:10:00-sun:10:30"
  db_username = data.aws_ssm_parameter.uh_postgres_username.value
  db_password = data.aws_ssm_parameter.uh_postgres_db_password.value
  storage_encrypted = false
  multi_az = true //only true if production deployment
  publicly_accessible = false
  project_name = "platform apis"
}

/*    DMS SET UP    */
data "aws_ssm_parameter" "uh_hostname" {
   name = "/uh-api/live-server/hostname"
}
data "aws_ssm_parameter" "uh_postgres_hostname" {
   name = "/uh-api/production/postgres-hostname"
}

/*Target Endpoint*/
module "target_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = "uh_mirror"
    dms_endpoint_identifier = "target-uh-endpoint"
    engine_name = "postgres"
    database_port = 5303
    db_username = data.aws_ssm_parameter.uh_postgres_username.value
    db_password = data.aws_ssm_parameter.uh_postgres_db_password.value
    db_server = data.aws_ssm_parameter.uh_postgres_hostname.value
    ssl_mode = "none"
    endpoint_type = "target"
    environment_name = "production"
    project_name = "resident-information-api"
}

/*UHT credentials*/
data "aws_ssm_parameter" "uht_db_name" {
    name = "/uh-api/live-server/uht_db_name"
}
data "aws_ssm_parameter" "uht_username" {
    name = "/uh-api/live-server/uht-username"
}
data "aws_ssm_parameter" "uht_password" {
    name = "/uh-api/live-server/uht-password"
}
/*UHT Source endpoint*/
module "uht_source_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = data.aws_ssm_parameter.uht_db_name
    dms_endpoint_identifier = "source-uh-uht-endpoint"
    engine_name = "sqlserver"
    database_port = 1433
    db_username = data.aws_ssm_parameter.uht_username.value
    db_password = data.aws_ssm_parameter.uht_password.value
    db_server = data.aws_ssm_parameter.uh_hostname.value
    ssl_mode = "none"
    endpoint_type = "source"
    environment_name = "production"
    project_name = "resident-information-api"
}

/*UHT replication task*/
module "dms_uht_setup_production" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_replication_task"
    environment_name = "production" //used for resource tags
    project_name = "resident-information-api" //used for resource tags
    //dms task set up
    replication_instance_arn = "arn:aws:dms:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:rep:65CJ5HE2DMCUW5X6EPKTKUDVWA"
    migration_type = "full-load-and-cdc"
    replication_task_indentifier = "uh-uht-api-dms-task"
    task_settings = file("${path.module}/task_settings.json")
    task_table_mappings = file("${path.module}/uht_selection_rules.json")
    //replication endpoints
    source_endpoint_arn = module.uht_source_dms_endpoint.dms_endpoint_arn
    target_endpoint_arn = module.target_dms_endpoint.dms_endpoint_arn
}

/*UHW credentials*/
data "aws_ssm_parameter" "uhw_username" {
    name = "/uh-api/live-server/uhw-username"
}
data "aws_ssm_parameter" "uhw_password" {
    name = "/uh-api/live-server/uhw-password"
}
data "aws_ssm_parameter" "uhw_db_name" {
    name = "/uh-api/live-server/uhw_db_name"
}
/*UHT Source endpoint*/
module "uhw_source_dms_endpoint" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_endpoint"
    database_name = data.aws_ssm_parameter.uhw_db_name
    dms_endpoint_identifier = "source-uh-uhw-endpoint"
    engine_name = "sqlserver"
    database_port = 1433
    db_username = data.aws_ssm_parameter.uhw_username.value
    db_password = data.aws_ssm_parameter.uhw_password.value
    db_server = data.aws_ssm_parameter.uh_hostname.value
    ssl_mode = "none"
    endpoint_type = "source"
    environment_name = "production"
    project_name = "resident-information-api"
}
/* UHW replication task */
module "dms_uhw_setup" {
    source = "github.com/LBHackney-IT/aws-dms-terraform.git//dms_replication_task"
    environment_name = "production" //used for resource tags
    project_name = "resident-information-api" //used for resource tags
    //dms task set up
    replication_instance_arn = "arn:aws:dms:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:rep:65CJ5HE2DMCUW5X6EPKTKUDVWA"
    migration_type = "full-load-and-cdc"
    replication_task_indentifier = "uh-uhw-api-dms-task"
    task_settings = file("${path.module}/task_settings.json")
    task_table_mappings = file("${path.module}/uhw_selection_rules.json")
    //replication endpoints
    source_endpoint_arn = module.uhw_source_dms_endpoint.dms_endpoint_arn
    target_endpoint_arn = module.target_dms_endpoint.dms_endpoint_arn
}
