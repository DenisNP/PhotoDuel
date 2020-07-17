<template>
    <div>
        <div v-if="duels.length === 0" class="empty-duels">
            <div>Здесь будут отображаться все дуэли, в которых вы приняли участие.</div>
            <f7-button class="call-btn" @click="$store.commit('setTab', 1)">
                Вызвать на дуэль
            </f7-button>
        </div>
        <div v-if="duels.length > 0" class="duels-list">
            <duel class="duel-block" v-for="d in duels" :duel="d" :key="d.id"/>
        </div>
    </div>
</template>

<script>
import Duel from '../components/Duel.vue';

export default {
    name: 'TabDuels',
    components: { Duel },
    computed: {
        duels() {
            return this.$store.state.myDuels;
        },
    },
};
</script>

<style scoped>
    .empty-duels {
        text-align: center;
        padding: 170px 50px 50px 50px;
        color: #888;
    }

    .call-btn {
        margin-top: 20px;
    }

    .duels-list {
        padding: 10px 0 0 0;
    }

    .duel-block {
        margin-bottom: 20px;
    }
</style>
