# Tippy Theme

## SASS

We modified https://github.com/mazipan/bulma-admin-dashboard-template to our needs. Its dependencies (bulma, bulma-dracula) are included.

### Modifications

* Bulma navbar breakpoint was changed to suite desktop only behavior:

```html
$navbar-breakpoint: 10px !default
// $navbar-breakpoint: $desktop !default
```

## Generate CSS

- Install webcompiler from https://github.com/excubo-ag/WebCompiler.
- Run this to regenerate `wwwroot/css/theme.css`:
```shell
webcompiler theme.scss -o ../css -z disable -m disable
```

