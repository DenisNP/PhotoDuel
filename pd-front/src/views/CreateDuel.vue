<template>
    <f7-page class="create-duel-container">
        <f7-navbar class="left-title" back-link :title="duel ? 'Принять вызов' : 'Создать вызов'"/>
        <challenge class="cd-challenge" :challenge-id="challengeId"/>
        <div class="photo-container">
            <vue-croppie
                ref="croppieRef"
                :enableOrientation="true"
                :enableResize="false"
                :showZoomer="false"
                :viewport="{width: getWidth(), height: getWidth()}"
            />
            <div class="rotate-button" v-if="imageSelected" @click="rotateImage">
                <f7-icon material="rotate_right"/>
            </div>
        </div>
        <div class="buttons-block">
            <label>
                <span class="button-dummy" v-if="!imageSelected"/>
                <f7-button
                    :href="false"
                    external
                    large
                    fill
                    icon-material="camera_alt"
                    class="my-icon"
                >
                    Выбрать фото
                </f7-button>
                <input
                    @change="photoSelected"
                    type="file"
                    accept="image/*"
                    style="display:none;"
                />
            </label>
            <f7-button
                :href="false"
                external
                large
                fill
                icon-material="local_fire_department"
                class="my-icon margin-top-btn"
                :disabled="!imageSelected"
                @click="createDuel"
            >
                Создать вызов
            </f7-button>
            <f7-button
                :href="false"
                external
                large
                icon-material="clear"
                class="my-icon"
                @click="$f7.views.main.router.back()"
            >
                Отмена
            </f7-button>
        </div>
    </f7-page>
</template>

<script>
import Challenge from '../components/Challenge.vue';

export default {
    name: 'CreateDuel',
    data() {
        return {
            imageSelected: false,
        };
    },
    computed: {
        duel() {
            const duel = this.$store.state.myDuels.find((d) => d.id === this.id);
            const { publicDuel } = this.$store.state.user;
            return duel || ((publicDuel && publicDuel.id === this.id) ? publicDuel : null);
        },
        challengeId() {
            return Number.parseInt(this.duel ? this.duel.challengeId : this.id, 10);
        },
    },
    methods: {
        getWidth() {
            return document.documentElement.clientWidth - 60;
        },
        rotateImage() {
            if (!this.imageSelected) return;
            this.$refs.croppieRef.rotate(-90);
        },
        photoSelected(e) {
            const files = e.target.files || e.dataTransfer.files;
            if (!files.length) return;

            const reader = new FileReader();
            reader.onload = (fr) => {
                this.$refs.croppieRef.bind({
                    url: fr.target.result,
                });
                this.imageSelected = true;
            };

            reader.readAsDataURL(files[0]);
        },
        async createDuel() {
            // TODO loader
            const file = await this.$refs.croppieRef.result({
                type: 'base64',
                size: { width: 750, height: 750 },
                format: 'jpeg',
            });
            console.log(file);
            // TODO await call api
            this.$f7.views.main.router.back();
        },
    },
    props: {
        id: {
            type: String,
            required: true,
        },
    },
    components: { Challenge },
};
</script>

<style>
    .md .left-title .navbar-inner .title {
        margin-left: 0;
    }

    .my-icon > .icon {
        margin-right: 3px;
        transform: translateY(-2px);
    }
</style>

<style scoped>
    .create-duel-container {
        background: white;
    }

    .cd-challenge {
        margin: 15px 30px 15px 30px;
    }

    .photo-container {
        width: calc(100vw - 40px);
        height: calc(100vw - 40px);
        margin-left: 20px;
        position: relative;
    }

    .rotate-button {
        position: absolute;
        right: 10px;
        bottom: 10px;
        background-color: #00000066;
        z-index: 1;
        padding: 4px;
        border-radius: 7px;
        color: white;
        cursor: pointer;
    }

    .buttons-block {
        margin: 20px 20px 0 20px;
        position: relative;
    }

    .button-dummy {
        width: calc(100vw - 40px);
        height: calc(100vw - 40px);
        position: absolute;
        top: calc(-100vw + 20px);
        background: rgba(0, 0, 0, 0.1);
        z-index: 1;
    }

    .margin-top-btn {
        margin-top: 10px;
    }
</style>
