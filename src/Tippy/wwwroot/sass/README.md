# Tippy Theme

## SASS

We modified https://github.com/mazipan/bulma-admin-dashboard-template to our needs. Its dependencies (bulma, bulma-dracula) are included.

## Generate CSS

- Install webcompiler from https://github.com/excubo-ag/WebCompiler.
- Run this to regenerate `wwwroot/css/theme.css`:
```shell
webcompiler theme.scss -o ../css -z disable -m disable
```

