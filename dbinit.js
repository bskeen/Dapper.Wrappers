const { Client } = require('pg');
const sql = require('mssql');

const pgclient = new Client({
    host: 'localhost',
    port: 5432,
    user: 'dappertest',
    password: process.env.POSTGRES_PASSWORD,
    database: 'postgres'
});

const sqlConfig = {
    server: 'localhost',
    user: 'sa',
    password: process.env.SQL_SERVER_PASSWORD,
    database: 'master',
    options: {
        trustServerCertificate: true
    }
};

pgclient.connect();

const pgDbCheck = 'SELECT COUNT(*) as dbCount FROM pg_database WHERE datname = \'dapperwrapperstest\'';
const pgDatabase = 'CREATE DATABASE dapperwrapperstest WITH OWNER dappertest;';

(async function() {
    await pgclient.connect();
    const dbCount = await pgclient.query(pgDbCheck);
    if (+dbCount.rows[0].dbcount === 0) {
        await pgclient.query(pgDatabase);
    }

    await pgclient.end();
})();

const sqlDbCreate = 'IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = \'DapperWrappersTest\') CREATE DATABASE DapperWrappersTest;';

(async function() {
    try {
        let pool = await sql.connect(sqlConfig);
        await pool.request().query(sqlDbCreate);
        await sql.close();
    } catch (err) {
        await sql.close();
        throw err;
    }
})();
