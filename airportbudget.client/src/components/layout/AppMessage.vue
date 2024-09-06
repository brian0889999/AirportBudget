<template>
 <div>
     <v-card elevation="6"
             width="400"
             class="d-flex flex-column message-card"
             color="transparent">
         <v-slide-y-reverse-transition tag="div"
                                       class="d-flex flex-column message-box"
                                       group
                                       hide-on-leave>
             <AppMessageItem v-for="message in messages"
                             :key="message.id"
                             :type="message.type"
                             :timeout="timeout"
                             class="ma-1"
                             border="start"
                             @close="deleteMessage(message.id)">
                 <small>{{ formatTime(message.time) }}</small>
                 <div>{{ message.text }}</div>
             </AppMessageItem>
         </v-slide-y-reverse-transition>
     </v-card>
 </div>
</template>

<script setup lang="ts">
    import { ref, computed } from 'vue';
    import { useMessageStore } from '@/store/message';
    import AppMessageItem from '@/components/layout/AppMessageItem.vue';
    import { format } from 'date-fns';

    const messageStore = useMessageStore();

    const timeout = ref(5000);

    const messages = computed(() => messageStore.messages);

    const deleteMessage = (id: number) => {
        messageStore.delMessage(id);
    };

    const emptyMessages = () => {
        messageStore.$reset();
    };

    const formatTime = (date: Date) => {
        return format(date, 'yyyy-MM-dd HH:mm:ss');
    };
</script>

<style lang="scss" scoped>
    $footer-height: 30px;
    $app-bar-height: 60px;

    .message-card {
        position: fixed;
        z-index: 210;
        right: 15px;
        bottom: calc(#{$footer-height} + 5px);
        max-height: calc(100vh - #{$footer-height} - #{$app-bar-height} - 10px);
        visibility: hidden;
    }

    .message-box {
        overflow-y: visible;
        visibility: visible;
    }
</style>