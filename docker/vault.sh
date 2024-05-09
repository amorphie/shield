sleep 5 &&
curl -X POST 'http://bbt-shield-vault:8200/v1/secret/data/amorphie-shield' -H "Content-Type: application/json" -H "X-Vault-Token: admin" -d '{ "data": {"shielddb":"Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres;Include Error Detail=true;"} }'
