<template>
    <f7-app :params="f7params">
        <transition name="popup">
            <div class="onboarding" v-if="$store.state.showOnboarding">
                <f7-swiper pagination>
                    <f7-swiper-slide>
                        <div class="onboarding-slide">
                            <div class="ob-pic">
                                <img src="./assets/ondoarding_0.jpg">
                            </div>
                            <div class="ob-text">
                                Каждый день вы будете получать на выбор несколько <b>заданий</b> на
                                фотографирование.
                                <span class="ob-small">
                                    Там будут приключения, абстрактное искусство,
                                    челленджи и много всего ещё.
                                </span>
                            </div>
                            <f7-button fill @click="slideNext">Далее</f7-button>
                        </div>
                    </f7-swiper-slide>
                    <f7-swiper-slide>
                        <div class="onboarding-slide">
                            <div class="ob-pic">
                                <img src="./assets/ondoarding_1.jpg">
                            </div>
                            <div class="ob-text">
                                Сделав фото по заданию, вы сможете <b>вызвать на дуэль</b>
                                кого-нибудь.
                                <span class="ob-small">
                                    Есть вызовы для друзей или соревнование
                                    со случайным незнакомцем.
                                </span>
                            </div>
                            <f7-button fill @click="slideNext">Далее</f7-button>
                        </div>
                    </f7-swiper-slide>
                    <f7-swiper-slide>
                        <div class="onboarding-slide">
                            <div class="ob-pic">
                                <img src="./assets/ondoarding_2.jpg">
                            </div>
                            <div class="ob-text">
                                Когда вызов примут, вы публикуете
                                <b>голосование</b> в Историях.
                                <span class="ob-small">
                                    Выбирать победителя смогут все ваши
                                    друзья, даже те, кого нет в приложении!
                                </span>
                            </div>
                            <f7-button fill @click="start">Хорошо!</f7-button>
                        </div>
                    </f7-swiper-slide>
                </f7-swiper>
            </div>
        </transition>
        <f7-view :push-state="true" main url="/"/>
    </f7-app>
</template>

<script>
import Main from './views/Main.vue';
import CreateDuel from './views/CreateDuel.vue';

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
                    {
                        path: '/duel/:id',
                        component: CreateDuel,
                    },
                ],
                dialog: {
                    buttonOk: 'Да',
                    buttonCancel: 'Отмена',
                },
            },
        };
    },
    computed: {
        isLoading() {
            return this.$store.state.isLoading;
        },
        noInternet() {
            return this.$store.state.noInternet;
        },
    },
    methods: {
        slideNext() {
            const sw = this.$f7.swiper.get();
            if (sw) sw.slideNext();
        },
        start() {
            this.$store.dispatch('saveOnboarding');
        },
    },
    watch: {
        isLoading(l) {
            if (l) this.$f7.preloader.show();
            else this.$f7.preloader.hide();
        },
        noInternet(n) {
            if (n) {
                this.$f7.toast.create({
                    text: 'Проблемы с интернетом, попробуйте позже!',
                    position: 'center',
                    cssClass: 'my-text-center',
                    closeTimeout: 3000,
                }).open();
            }
        },
    },
};
</script>

<style>
    body {
        -webkit-user-select: none;
        user-select: none;
        overscroll-behavior-y: none;
    }

    html,
    body {
        position: fixed;
        overflow: hidden;
    }

    /* Custom color theme */
    :root {
        --f7-theme-color: #EB643A;
        --f7-theme-color-rgb: 235, 100, 58;
        --f7-theme-color-shade: #e54817;
        --f7-theme-color-tint: #ef815f;
        /*--f7-preloader-modal-bg-color: rgba(220, 220, 220, 1)!important;*/
    }

    .preloader[class*="color-"] {
        /*--f7-preloader-color: var(--f7-theme-color-shade)!important;*/
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

    .md {
        --f7-navbar-shadow-image: none;
        --f7-toolbar-bottom-shadow-image: none;
        --f7-navbar-font-size: 17px;
        --f7-navbar-title-font-weight: 600;
        --f7-navbar-title-text-align: center;
        --f7-font-family: -apple-system, SF Pro Text, SF UI Text, system-ui, Helvetica Neue,
            Helvetica, Arial, sans-serif;
        --f7-line-height: 1.4;
        --f7-text-color: #000;
    }

    .md .navbar-inner .title {
        margin-left: auto;
        margin-right: auto;
    }

    .my-text-center {
        text-align: center;
    }

    .onboarding {
        position: absolute;
        top: 0;
        left: 0;
        width: 100vw;
        height: 100vh;
        background: rgba(0, 0, 0, 0.2);
        z-index: 5001;
    }

    .onboarding-slide {
        width: calc(100vw - 80px);
        height: calc(100vh - 120px - env(safe-area-inset-top) - env(safe-area-inset-bottom));
        max-height: 140vw;
        margin-top: calc(20px + env(safe-area-inset-top));
        margin-left: 20px;
        background: white;
        border-radius: 15px;
        padding: 40px 20px;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }

    .popup-enter-active, .popup-leave-active {
        transition: all .3s;
    }

    .popup-enter, .popup-leave-to {
        transform: scale(0.1);
        opacity: 0;
    }

    .ob-text {
        text-align: center;
        font-size: 16px;
    }

    .ob-small {
        font-size: 14px;
        margin-top: 15px;
        margin-bottom: 5px;
        display: block;
    }

    .ob-pic {
        width: calc(100vw - 100px);
        margin-left: auto;
        margin-right: auto;
        border-radius: 10px;
        overflow: hidden;
        line-height: 0;
    }

    .ob-pic > img {
        width: 100%;
    }
</style>
