﻿# Cluster Node Health Check

A health check sends an HTTP request to `/databases/[Database Name]/stats` endpoint. 
If the request is successful, it will reset node failure counters which will cause the client to try sending operations to that specific node again.

### When Does it Trigger?
Any time a low-level [operation](../operations/what-are-operations) fails to connect to a node, the client spawns a health check thread for that particular node. 
The thread will periodically ping the not responding server, until it gets a proper response.
The frequency of pinging the not responding server will start from 100ms and gradually (with steps of 100ms) will increase until it reaches 5sec interval.