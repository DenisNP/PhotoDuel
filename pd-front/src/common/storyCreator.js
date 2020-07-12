import html2canvas from 'html2canvas';
import { drawImage, getAppId } from './utils';
import soloBg from '../assets/story_single.jpg';
import voteBg from '../assets/story_bg.jpg';

const cWidth = 830;
const cHeight = 142;
const iSize = 850;
const ivSize = 750;
const allWidth = 1150;
const allHeight = 1900;

const createSoloCanvas = async (imageUrl, challengeElement) => {
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    await drawImage(soloBg, 0, 0);
    await drawImage(imageUrl, (allWidth - iSize) / 2, 870 - iSize / 2, iSize, iSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (allWidth - cWidth) / 2, 266 - cHeight / 2);
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
                    blob: 'data:image/png;base64,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',
                    transform: {
                        rotation: 0,
                        relation_width: 0.68,
                        translation_x: 0,
                        translation_y: 0.3,
                        gravity: 'center',
                    },
                    clickable_zones: [
                        {
                            action_type: 'link',
                            action: {
                                link: `https://vk.com/app${getAppId()}#${duelId}`,
                            },
                            // eslint-disable-next-line max-len
                            clickable_area: [{ x: 0, y: 0 }, { x: 9999, y: 0 }, { x: 9999, y: 9999 }, { x: 0, y: 9999 }],
                        },
                    ],
                    original_width: allWidth,
                    original_height: allHeight,
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
    await drawImage(firstImgUrl, (allWidth - ivSize) / 2, 430 - ivSize / 2, ivSize, ivSize);
    await drawImage(secondImgUrl, (allWidth - ivSize) / 2, 1470 - ivSize / 2, ivSize, ivSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (allWidth - cWidth) / 2, 970 - cHeight / 2);
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
                    blob: 'data:image/png;base64,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',
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
                            clickable_area: [{ x: 0, y: 0 }, { x: allWidth, y: 0 }, { x: allWidth, y: allHeight / 2 }, { x: 0, y: allHeight / 2 }],
                        },
                        {
                            action_type: 'link',
                            action: {
                                link: `https://vk.com/app${appId}#${duelId}_vote_o`,
                            },
                            // eslint-disable-next-line max-len
                            clickable_area: [{ x: 0, y: allHeight / 2 }, { x: allWidth, y: allHeight / 2 }, { x: allWidth, y: allHeight }, { x: 0, y: allHeight }],
                        },
                    ],
                    original_width: allWidth,
                    original_height: allHeight,
                    can_delete: false,
                },
            },
        ],
    };
};
