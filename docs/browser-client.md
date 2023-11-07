# Browser client

The browser client can be found in the [web directory](../@xoutput).

## Prerequisites

The web application requires Node 20 to build it, you can [download it](https://nodejs.org/en/download/).

[`pnpm`](https://pnpm.io/) is used instead of `npm`, so that should be installed.

```shell
npm install --global pnpm
```

Install dependencies and build the application.

```shell
pnpm --recursive install
pnpm --recursive run build
```

For development, there is a development server and watch builds.

```shell
pnpm --recursive run watch
```

