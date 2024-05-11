# Cyex
Find vulnerable packages in project definition files.

## GitHub Access Token
- Development - setup `GitHubAccessToken` in `appsettings.Development.json`.
- Otherwise - `GITHUB_ACCESS_TOKEN` in environment machine variable

## Building and Running
### Prerequisites
- .NET 8 SDK
- Docker
- Docker Compose

### Open directory
```sh
$ cd Cyex
```

### Build docker image
```sh
$ docker-compose build
```

### Run docker container
```sh
$ docker-compose up
```

### Stop docker container
```sh
$ docker-compose down
```

## Usage
Run `Cyex.http` example