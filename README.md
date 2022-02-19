# Unnecessary Roughness

## Build Status

[![CICD](https://github.com/JanneMattila/unnecessary-roughness/actions/workflows/CICD.yaml/badge.svg)](https://github.com/JanneMattila/unnecessary-roughness/actions/workflows/CICD.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Introduction

Unnecessary Roughness (later referred just by "UR") is based
on historical football game called [Calcio Storico](https://en.m.wikipedia.org/wiki/Calcio_Fiorentino).

## Working with UR

### How to create image locally

```powershell
# Build container image
docker build -t ur -f .\src\UR.Server\Dockerfile .

# Run container using command
docker run --rm -p "2001:80" -p "2002:443" ur:latest
```
