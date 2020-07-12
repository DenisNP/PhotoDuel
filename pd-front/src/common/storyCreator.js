import html2canvas from 'html2canvas';
import { drawImage, getAppId } from './utils';
import soloBg from '../assets/story_single.jpg';
import voteBg from '../assets/story_bg.jpg';
import stickerBtn from '../assets/join_button.png';
import stickerVote from '../assets/stories_sticker.png';

const cWidth = 830;
const cHeight = 142;
const iSize = 850;
const ivSize = 750;

const createSoloCanvas = async (imageUrl, challengeElement) => {
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    await drawImage(soloBg, 0, 0);
    await drawImage(imageUrl, (1000 - iSize) / 2, 870 - iSize / 2, iSize, iSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (1000 - cWidth) / 2, 266 - cHeight / 2);
    return canvas.toDataURL('image/jpeg');
};

export const createSoloStory = async (imageUrl, challengeElement, duelId) => {
    const dataUrl = await createSoloCanvas(imageUrl, challengeElement);
    return {
        background_type: 'image',
        blob: dataUrl,
        locked: true,
        stickers: [
            {
                sticker_type: 'renderable',
                sticker: {
                    content_type: 'image',
                    url: `${window.location.origin}/${stickerBtn}`,
                    transform: {
                        rotation: 0,
                        relation_width: 0,
                        translation_x: 0,
                        translation_y: 0.7,
                        gravity: 'center',
                    },
                    clickable_zones: [
                        {
                            action_type: 'link',
                            action: {
                                link: `https://vk.com/app${getAppId()}#${duelId}`,
                            },
                            // eslint-disable-next-line max-len
                            clickable_area: [{ x: 13, y: 134 }, { x: 664, y: 134 }, { x: 664, y: 260 }, { x: 13, y: 260 }],
                        },
                    ],
                    original_width: 1000,
                    original_height: 1900,
                    can_delete: true,
                },
            },
        ],
    };
};

const createVoteCanvas = async (firstImgUrl, secondImgUrl, challengeElement) => {
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    await drawImage(voteBg, 0, 0);
    await drawImage(firstImgUrl, (1000 - ivSize) / 2, 430 - ivSize / 2, ivSize, ivSize);
    await drawImage(secondImgUrl, (1000 - ivSize) / 2, 1470 - ivSize / 2, ivSize, ivSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (1000 - cWidth) / 2, 970 - cHeight / 2);
    return canvas.toDataURL('image/jpeg');
};

export const createVoteStory = async (firstImgUrl, secondImgUrl, challengeElement, duelId) => {
    const dataUrl = await createVoteCanvas(firstImgUrl, secondImgUrl, challengeElement);
    const appId = getAppId();
    return {
        background_type: 'image',
        blob: dataUrl,
        locked: true,
        stickers: [
            {
                sticker_type: 'renderable',
                sticker: {
                    content_type: 'image',
                    url: `${window.location.origin}/${stickerVote}`,
                    transform: {
                        rotation: 0,
                        relation_width: 1,
                        translation_x: 0,
                        translation_y: 0,
                        gravity: 'center',
                    },
                    clickable_zones: [
                        {
                            action_type: 'link',
                            action: {
                                link: `https://vk.com/app${appId}#${duelId}_vote_c`,
                            },
                            // eslint-disable-next-line max-len
                            clickable_area: [{ x: 0, y: 0 }, { x: 1000, y: 0 }, { x: 1000, y: 950 }, { x: 0, y: 950 }],
                        },
                        {
                            action_type: 'link',
                            action: {
                                link: `https://vk.com/app${appId}#${duelId}_vote_o`,
                            },
                            // eslint-disable-next-line max-len
                            clickable_area: [{ x: 0, y: 950 }, { x: 1000, y: 950 }, { x: 1000, y: 1900 }, { x: 0, y: 1900 }],
                        },
                    ],
                    original_width: 1000,
                    original_height: 1900,
                    can_delete: false,
                },
            },
        ],
    };
};
