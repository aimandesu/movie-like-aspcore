# 📇 MOVIE LIKE ASPCORE NET

A Movie/Series API - currently in Development

## ✨ Tech Stack

- **ASP.NET Core** 9.0.0
- **C#** 13
- **Docker** 28.0.4

## API Access

- http://aimanafiq.runasp.net/api/{REST_ENDPONT}

## Docker Access

1. If you use windows, please install WSL and Docker Desktop
2. If you use Linux or MAC, please install docker and docker compose v2
3. Run docker compose up --build

### REST ENDPOINT

- series - Full CRUD support
- categories - No UPDATE support
- tags - No UPDATE support
- episode/id - No UPDATE support
- video/id - No UPDATE, READ ALL
- user - Register, Login
- comments - No UPDATE support

### Instructions using POSTMAN

Put URL at front.

If you are using localhost follow the Docker Access instructions and put localhost:9260

- for example, localhost:9260/api/series

if you are using the API access, put http://aimanafiq.runasp.net/api/

- for example, http://aimanafiq.runasp.net/api/series

#### Series

GET - /api/series - /api/series/:slug

POST - /api/series
DELETE - /api/series/:id
UPDATE - /api/series/:id

### Category

GET - /api/categories - /api/categories/:id

POST - /api/categories
DELETE - /api/categories/:id

and others... the same. Too Lazy rn, will update later :/
maybe ill do ui for all my api later........

## Screenshots

### Erd

<div>
  <img src="erd.png" width="100%" alt="Screenshot 1">
</div>
