import Vue from 'vue';
import Vuex from 'vuex';
import mutations from './mutations';
import actions from './actions';
import getters from './getters';

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
                        name: 'Денис',
                        photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
                    },
                    image: 'http://lorempixel.com/500/500/',
                    story: '',
                    voters: [],
                    time: 1594072034500,
                },
                opponent: {
                    user: {
                        id: '463377',
                        name: 'Денис',
                        photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
                    },
                    image: 'http://lorempixel.com/600/600/',
                    story: '',
                    voters: [],
                    time: 1594072034500,
                },
                timeStart: 0,
                timeFinish: 1594153225000,
                challengeId: 1001,
            },
        ],
        pantheon: [
            {
                user: {
                    id: '463377',
                    name: 'Денис',
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
                color: '#ff0000',
                challenges: [
                    {
                        id: 1001,
                        icon: 'settings_applications',
                        name: 'Задание 1',
                        description: 'Выполнить задание 1 Выполнить задание 1 Выполнить задание 1',
                    },
                    {
                        id: 1002,
                        icon: 'settings_backup_restore',
                        name: 'Задание 2',
                        description: 'Выполнить задание 2 Выполнить задание 2 Выполнить задание 2 Выполнить задание 2 ',
                    },
                    {
                        id: 1003,
                        icon: 'settings_backup_restore',
                        name: 'Задание 3',
                        description: 'Выполнить задание 3',
                    },
                    {
                        id: 1004,
                        icon: 'settings_backup_restore',
                        name: 'Задание 4',
                        description: 'Выполнить задание 4',
                    },
                    {
                        id: 1005,
                        icon: 'settings_backup_restore',
                        name: 'Задание 5',
                        description: 'Выполнить задание 5',
                    },
                    {
                        id: 1006,
                        icon: 'settings_backup_restore',
                        name: 'Задание 6',
                        description: 'Выполнить задание 6',
                    },
                    {
                        id: 1007,
                        icon: 'settings_backup_restore',
                        name: 'Задание 7',
                        description: 'Выполнить задание 7',
                    },
                ],
            },
            {
                id: 2,
                name: 'Категория 2',
                color: '#00ff00',
                challenges: [
                    {
                        id: 2001,
                        icon: 'rowing',
                        name: 'Задание 3',
                        description: 'Выполнить задание 3 Выпол задание 3 Выполнить задание 3',
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
                color: '#0000ff',
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
                1005,
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
                        name: 'Пацан',
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
            },
            id: '463377',
            name: 'Денис',
            photo: 'https://sun9-1.userapi.com/c858416/v858416480/1ae5a1/okte4guqxFM.jpg?ava=1',
        },
    },
    getters,
    mutations,
    actions,
});
