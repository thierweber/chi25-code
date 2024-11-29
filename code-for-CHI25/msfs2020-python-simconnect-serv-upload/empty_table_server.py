#Imports for SERVER CONNECTION (OWN CODE)
import psycopg2
from psycopg2.extensions import AsIs
#SERVER CONNECTION CREDENTIALS (OWN CODE)
db_credentials = {
    "user": "XXXX",
    "password": "XXXX",
    "host": "XXXX",
    "port": "XXXX",
    "dbname": "XXXXXX"
}

def empty_table(schema, table):
    #This function is used to empty a table on the server.
    try:
        conn = psycopg2.connect(**db_credentials)
        cur = conn.cursor()
        query = f'DELETE FROM {schema}.{table};'

        cur.execute(query)
        conn.commit()
        conn.close()
        print(f"Table {schema}.{table} has been emptied successfully.")
    except psycopg2.Error as e:
        print(f"Error emptying table: {e}")

empty_table('simconnect', 'simconnect2')
