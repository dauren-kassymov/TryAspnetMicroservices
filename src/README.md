# TryAspnetMicroservices

### Run all microservices

`docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d`

#### Demo how to run PostgreSQL
`docker run -d --name test-pg -p 5433:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=DiscountDb timescale/timescaledb:2.2.0-pg13`