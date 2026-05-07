# Sitemap

## Public URLs

| URL | Controller | Action | View |
| --- | --- | --- | --- |
| `/` | Home | Index | `Views/Home/Index.cshtml` |
| `/Home/Index` | Home | Index | `Views/Home/Index.cshtml` |
| `/Home/Index` | Home | Index (POST) | `Views/Home/Index.cshtml` |
| `/Home/PlayAgain` | Home | PlayAgain (POST) | Redirects to `Home/Index` |
| `/Home/Error` | Home | Error | `Views/Shared/Error.cshtml` |
| `/Home/Privacy` | Home | Privacy | `Views/Home/Privacy.cshtml` |
| `/Band/Index` | Band | Index | `Views/Band/Index.cshtml` |
| `/bands` | Band | Index | `Views/Band/Index.cshtml` |
| `/Band/Details/{id}` | Band | Details | `Views/Band/Details.cshtml` |
| `/bands/{id}` | Band | Details | `Views/Band/Details.cshtml` |
| `/Album/Index` | Album | Index | `Views/Album/Index.cshtml` |
| `/albums` | Album | Index | `Views/Album/Index.cshtml` |
| `/Album/Details/{id}` | Album | Details | `Views/Album/Details.cshtml` |
| `/albums/{id}` | Album | Details | `Views/Album/Details.cshtml` |
| `/Genre/Index` | Genre | Index | `Views/Genre/Index.cshtml` |
| `/genres` | Genre | Index | `Views/Genre/Index.cshtml` |
| `/Genre/Details/{id}` | Genre | Details | `Views/Genre/Details.cshtml` |
| `/genres/{id}` | Genre | Details | `Views/Genre/Details.cshtml` |
| `/Song/Index` | Song | Index | `Views/Song/Index.cshtml` |
| `/songs` | Song | Index | `Views/Song/Index.cshtml` |
| `/Song/Details/{id}` | Song | Details | `Views/Song/Details.cshtml` |
| `/songs/{id}` | Song | Details | `Views/Song/Details.cshtml` |

## Notes

- The `Home/Index` page contains the daily quiz form and uses a POST back to the same action.
- The friendly plural routes for list and details pages are active via attribute routing.