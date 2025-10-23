#!/bin/bash

# Task 1 - Create file structure with given bash 

# Array of class names
classes=("NodeDto" 
         "LocationDto"
         "NodeConnectionDto" 
         "UserDto" 
         "RouteDto" 
         "NetworkMetricDto" 
         "NetworkHealthDto" 
         "RelayTransactionDto" )

requestClasses=("RegisterNodeRequest" 
                "UpdateNodeStatusRequest" 
                "CreateConnectionRequest" 
                "CalculateRouteRequest" 
                "RegisterUserRequest" 
                "LoginRequest")
                
responseClasses=("AuthResponse"
                 "PagedResponse"
                 "ApiResponse")

# Loop through and create each class
for class in "${classes[@]}"
do
    dotnet new class -n "$class" -o Domain
done

for class in "${requestClasses[@]}"
do 
  dotnet new class -n "$class" -o Requests
done

for class in "${responseClasses[@]}"
do
  dotnet new class -n "$class" -o Responses
done
