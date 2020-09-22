.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build housing-resident-information-api

.PHONY: serve
serve:
	docker-compose build housing-resident-information-api && docker-compose up housing-resident-information-api

.PHONY: shell
shell:
	docker-compose run housing-resident-information-api bash

.PHONY: test
test:
	docker-compose up test-database & docker-compose build housing-resident-information-api-test && docker-compose up housing-resident-information-api-test

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format
