# OMSWeb

## Description
This project is MVP WebAPI that do CRUD operations over Northwind database

## Interaction
Backend interacts with external world using RESTful, thus for every entity from database there are HTTP verbs through which you can add, update, delete, and read data.

## Problem and its solution
In this project I have faced problem of slow GET-verb response for some entities that have many records in database. I have solved this problem using two techniques:  
- Caching;  
- Pagination;

Caching helped me to optimize all GET requests but with help of pagination request become even faster. I am sure that you questioning yourself how I have dealt with problem of cache invalidation and saving state of pagination in order to make "give next page"/"give previous page" action. You can find answers to this question in my repository :)
