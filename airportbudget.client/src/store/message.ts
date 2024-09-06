import { defineStore } from 'pinia';

interface State {
    messages: MessageState[];
    messageCount: number;
}
interface MessageState {
    id: number,
    type: 'info' | 'error' | 'success' | 'warning',
    text: string,
    time: Date,
}

export const useMessageStore = defineStore('message', {
    state: (): State => ({
        messages: [],
        messageCount: 0,
    }),
    actions: {
        addMessage(text: string, type: MessageState['type']) {
            this.messages.push({
                id: this.messageCount++,
                type: type,
                text: text,
                time: new Date(),
            })
        },
        delMessage(id: number) {
            const index = this.messages.findIndex((m) => m.id === id)
            if (index !== -1) {
                this.messages.splice(index, 1)
            }
        },
    }
});

export const Message = {
    info: (text: string) => useMessageStore().addMessage(text, 'info'),
    success: (text: string) => useMessageStore().addMessage(text, 'success'),
    warning: (text: string) => useMessageStore().addMessage(text, 'warning'),
    error: (val: any) => {
        let text = ''
        if (typeof val === 'string') {
            text = val;
        } else if (val instanceof Error) {
            text = val.message;
        } else {
            text = JSON.stringify(val);
        }
        useMessageStore().addMessage(text, 'error');
    },
}