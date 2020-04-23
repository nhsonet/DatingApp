export interface Message {
    id: number;
    senderId: number;
    senderKnowAs: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientKnownAs: string;
    recipientPhotoUrl: string;
    content: string;
    sentAt: Date;
    isRead: boolean;
    readAt: Date;
}