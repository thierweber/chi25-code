import math
import time
from typing import Dict, Optional, Any
from SimConnect import *

import time


import asyncpg
import asyncio
from functools import wraps

# This code is dependend on:
# https://github.com/hankhank10/MSFS2020-cockpit-companion?tab=readme-ov-file
# Author of this script:  XXXXX
# Only run this script (e.g., current File in PyCharm), instead of MSFS2020-cockpit-companion-master
delay_between_calls = 0.0333

# SERVER CONNECTION CREDENTIALS
db_credentials_psy = {
    "user": "XXXXX",
    "password": "XXX",
    "host": "XXX",
    "port": "XXXX",
    "dbname": "XXXX"
}
# SERVER CONNECTION CREDENTIALS FOR ASYNC
db_credentials_asy = {
    "user": "XXXX",
    "password": "XXX",
    "host": "XXXX",
    "database": "XXXX"
}#----------------------------------------------------------------------------------------------------------------------
async def est_conn():
  # Establish server connection
    conn = await asyncpg.connect(**db_credentials_asy)
    return conn
#----------------------------------------------------------------------------------------------------------------------
async def create_pool():
    return await asyncpg.create_pool(**db_credentials_asy)

#----------------------------------------------------------------------------------------------------------------------

def calls_per_second(func):
    """
    Function to count the number of calls of a function per second
    """

    @wraps(func)
    def wrapper(*args, **kwargs):
        if not hasattr(wrapper, 'start_time'):
            wrapper.count = 0
            wrapper.start_time = time.time()

        # Update call count
        wrapper.count += 1
        current_time = time.time()
        elapsed_time = current_time - wrapper.start_time

        if elapsed_time >= 1:
            print(f"Function '{func.__name__}' called {wrapper.count} times in {elapsed_time:.2f} seconds.")
            # Reset count and timestamp
            wrapper.count = 0
            wrapper.start_time = current_time

        return func(*args, **kwargs)

    @wraps(func)
    async def async_wrapper(*args, **kwargs):
        if not hasattr(async_wrapper, 'start_time'):
            async_wrapper.count = 0
            async_wrapper.start_time = time.time()

        # Update call count
        async_wrapper.count += 1
        current_time = time.time()
        elapsed_time = current_time - async_wrapper.start_time

        if elapsed_time >= 1:
            print(f"Function '{func.__name__}' called {async_wrapper.count} times in {elapsed_time:.2f} seconds.")
            # Reset count and timestamp
            async_wrapper.count = 0
            async_wrapper.start_time = current_time

        return await func(*args, **kwargs)

    if asyncio.iscoroutinefunction(func):
        return async_wrapper
    else:
        return wrapper


async def serverupdate( lat, lon, alt, heading, pitch, bank, speed, pool):
    """
    Function to upload sim variables to server.
    """
    async with pool.acquire() as conn:
        query = """INSERT INTO simconnect.simconnect3 (lat, lon, alt, heading, pitch, bank, speed) 
                   VALUES($1, $2, $3, $4, $5, $6, $7);"""

        result= await conn.execute(query,lat, lon, alt, heading, pitch, bank, speed)

    #await conn.commit()
    #await cur.close()
#----------------------------------------------------------------------------------------------------------------------

@calls_per_second
async def simcall(id, aq,  pool):
    """
    Function to read the simvariables via SimConnect API.
    """
    pitch_deg = None
    bank_deg = None
    lat = aq.get("PLANE_LATITUDE")
    lon = aq.get("PLANE_LONGITUDE")
    alt = round(aq.get("PLANE_ALTITUDE")) #in feet
    heading = round(aq.get("MAGNETIC_COMPASS"))
    pitch = aq.get("PLANE_PITCH_DEGREES")
    bank = aq.get("PLANE_BANK_DEGREES")
    if pitch is not None and bank is not None:
        pitch_deg = round(pitch * (180 / math.pi))
        bank_deg = round(bank * (180 / math.pi))
    airspeed = 1 #round(aq.get("AIRSPEED_INDICATED"))
    id = id + 1
    if pitch_deg is not None and bank_deg is not None:
        result = await serverupdate( lat, lon, alt, heading, pitch_deg, bank_deg, airspeed,  pool)
    #print("serverupdate result:", result)

    return id

#----------------------------------------------------------------------------------------------------------------------
# Main loop to continuously call the function
async def main():


    # Create simconnection
    sm = SimConnect()
    #ae = AircraftEvents(sm)
    aq = AircraftRequests(sm, _time=600)
    # instantiate ID
    id = 0
    pool = await create_pool()
    #conn = await est_conn()p
    try:
        while True:
            tasks = [simcall(id, aq, pool) for i in range(50)]  # Adjust the range as needed
            result = await asyncio.gather(*tasks)

    except asyncio.CancelledError:
        # Handle any cleanup here if you get cancelled (e.g., through a shutdown signal)
        print("Cancelled the loop")
    except Exception as e:
        # General exception handling, log or handle errors as appropriate
        print(f"An error occurred: {e}")
    finally:
        # Cleanup actions (if any) before closing, like closing database connections
        print("Cleanup and exit")

# Ensure asyncio.run is called appropriately
if __name__ == "__main__":
    asyncio.run(main())

