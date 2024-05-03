#!/bin/bash
export PATH="$PATH:/home/fran/.dotnet/tools"
dotnet-ef dbcontext scaffold "Name=ConnectionStrings:MySqlDb" MySql.EntityFrameworkCore -o Models --force --project WebApp
dotnet-ef dbcontext scaffold "Name=ConnectionStrings:MySqlDb" MySql.EntityFrameworkCore -o Models --force --project WebApi
