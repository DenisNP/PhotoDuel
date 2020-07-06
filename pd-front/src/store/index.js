import Vue from 'vue';
import Vuex from 'vuex';
import mutations from './mutations';
import actions from './actions';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        myDuels: [
            {
                id: 'lcv5UlIx',
                status: 'Created',
                isPublic: false,
                creator: {
                    user: {
                        id: '463377',
                        name: 'Денис Пешехонов',
                        photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
                    },
                    image: 'http://lorempixel.com/500/500/',
                    story: '',
                    voters: [],
                    time: 1594072034500,
                },
                opponent: null,
                timeStart: 0,
                timeFinish: 0,
                challengeId: 1001,
                challengeText: '',
            },
        ],
        pantheon: [
            {
                user: {
                    id: '463377',
                    name: 'Денис Пешехонов',
                    photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
                },
                image: 'http://lorempixel.com/500/500/',
                challengeId: 3001,
            },
        ],
        categories: [
            {
                id: 1,
                name: 'Категория 1',
                challenges: [
                    {
                        id: 1001,
                        icon: 'settings_applications',
                        name: 'Задание 1',
                        description: 'Выполнить задание 1',
                    },
                    {
                        id: 1002,
                        icon: 'settings_backup_restore',
                        name: 'Задание 2',
                        description: 'Выполнить задание 2',
                    },
                ],
            },
            {
                id: 2,
                name: 'Категория 2',
                challenges: [
                    {
                        id: 2001,
                        icon: 'rowing',
                        name: 'Задание 3',
                        description: 'Выполнить задание 3',
                    },
                    {
                        id: 2002,
                        icon: 'settings_input_composite',
                        name: 'Задание 4',
                        description: 'Выполнить задание 4',
                    },
                ],
            },
            {
                id: 3,
                name: 'Категория 3',
                challenges: [
                    {
                        id: 3001,
                        icon: 'description',
                        name: 'Задание 5',
                        description: 'Выполнить задание 5',
                    },
                    {
                        id: 3002,
                        icon: 'event',
                        name: 'Задание 6',
                        description: 'Выполнить задание 6',
                    },
                ],
            },
        ],
        message: '',
        user: {
            lastShuffle: 1594074756509,
            shufflesLeft: 1,
            challengeIds: [
                1001,
                2002,
                3001,
            ],
            publicDuel: {
                id: 'zS3xkSJd',
                status: 'Created',
                isPublic: true,
                creator: {
                    user: {
                        id: '418997910',
                        name: 'Пацан Бот',
                        photo: 'https://vk.com/images/camera_200.png?ava=1',
                    },
                    image: 'http://lorempixel.com/500/500/',
                    story: '',
                    voters: [],
                    time: 1594074636205,
                },
                opponent: null,
                timeStart: 0,
                timeFinish: 0,
                challengeId: 2001,
                challengeText: '',
            },
            id: '463377',
            name: 'Денис Пешехонов',
            photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
        },
    },
    mutations,
    actions,
});
