<template>
    <f7-link
        :external="!noLink"
        class="user"
        :href="noLink ? '' : `https://vk.com/id${user.id}`"
        target="_blank"
    >
        <div class="avatar">
            <img :src="user.photo"/>
        </div>
        <div class="name">
            {{user.name}}
        </div>
    </f7-link>
</template>

<script>
export default {
    name: 'User',
    props: {
        user: {
            type: Object,
            required: true,
        },
        noLink: {
            type: Boolean,
            default: false,
        },
    },
};
</script>

<style scoped>
    .user {
        display: flex;
        align-items: center;
        background: transparent;
        cursor: pointer;
        justify-content: flex-start;
    }

    .avatar {
        width: 30px;
        min-width: 30px;
        height: 30px;
        border-radius: 50%;
        overflow: hidden;
    }

    .avatar > img {
        width: 100%;
        height: 100%;
    }

    .name {
        font-size: 16px;
        color: black;
        margin-left: 10px;
        font-weight: 400;
    }
</style>
