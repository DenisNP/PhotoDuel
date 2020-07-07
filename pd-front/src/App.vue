<template>
    <f7-app :params="f7params">
        <f7-view :push-state="true" main url="/"/>
    </f7-app>
</template>

<script>
import Main from './views/Main.vue';

export default {
    data() {
        return {
            f7params: {
                name: 'Фотодуэль',
                routes: [
                    {
                        path: '/',
                        component: Main,
                    },
                ],
                dialog: {
                    buttonOk: 'Да',
                    buttonCancel: 'Отмена',
                },
            },
        };
    },
};
</script>

<style>
    /* Custom color theme */
    :root {
        --f7-theme-color: #EB643A;
        --f7-theme-color-rgb: 235, 100, 58;
        --f7-theme-color-shade: #e54817;
        --f7-theme-color-tint: #ef815f;
    }
    /* Invert navigation bars to fill style */
    :root,
    :root.theme-dark,
    :root .theme-dark {
        --f7-bars-bg-color: var(--f7-theme-color);
        --f7-bars-bg-color-rgb: var(--f7-theme-color-rgb);
        --f7-bars-translucent-opacity: 1.0;
        --f7-bars-text-color: #fff;
        --f7-bars-link-color: #fff;
        --f7-navbar-subtitle-text-color: rgba(255,255,255,0.85);
        --f7-bars-border-color: transparent;
        --f7-tabbar-link-active-color: #fff;
        --f7-tabbar-link-inactive-color: rgba(255,255,255,0.54);
        --f7-sheet-border-color: transparent;
        --f7-tabbar-link-active-border-color: #fff;
        --f7-navbar-height: 52px;
    }
    .appbar,
    .navbar,
    .toolbar,
    .subnavbar,
    .calendar-header,
    .calendar-footer {
        --f7-touch-ripple-color: var(--f7-touch-ripple-white);
        --f7-link-highlight-color: var(--f7-link-highlight-white);
        --f7-button-text-color: #fff;
        --f7-button-pressed-bg-color: rgba(255,255,255,0.1);
    }
    .navbar-large-transparent,
    .navbar-large.navbar-transparent {
        --f7-navbar-large-title-text-color: #000;

        --r: 235;
        --g: 100;
        --b: 58;
        --progress: var(--f7-navbar-large-collapse-progress);
        --f7-bars-link-color: rgb(
            calc(var(--r) + (255 - var(--r)) * var(--progress)),
            calc(var(--g) + (255 - var(--g)) * var(--progress)),
            calc(var(--b) + (255 - var(--b)) * var(--progress))
        );
    }
    .theme-dark .navbar-large-transparent,
    .theme-dark .navbar-large.navbar-transparent {
        --f7-navbar-large-title-text-color: #fff;
    }

    .my-text-center {
        text-align: center;
    }
</style>
