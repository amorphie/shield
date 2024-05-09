sleep 5 &&
curl -X POST 'http://bbt-template-vault:8200/v1/secret/data/amorphie-template' -H "Content-Type: application/json" -H "X-Vault-Token: admin" -d '{ "data": {"templatedb":"Host=localhost:5432;Database=TemplateDb;Username=postgres;Password=postgres;Include Error Detail=true;"} }'
