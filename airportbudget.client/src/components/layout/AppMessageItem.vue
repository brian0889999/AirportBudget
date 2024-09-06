<template>
     <v-alert v-bind="$attrs">
         <slot />
         <template #close>
             <v-btn icon
                    small
                    @click="closeAlert">
                 <v-icon small>mdi-close</v-icon>
             </v-btn>
         </template>
     </v-alert>
</template>

<script setup lang="ts">
    import { ref, computed, onMounted } from 'vue';

    const props = defineProps({
        modelValue: {
            type: Boolean,
            required: false
        },
        timeout: {
            type: Number,
            default: 5000,
        },
    });

    const emit = defineEmits(['update:modelValue', 'close']);

    onMounted(async () => {
       await startTimeout();
    });

    const startTimeout = async () => {
        window.setTimeout(() => {
            closeAlert();
        }, props.timeout)
    }

    const closeAlert = () => {
        emit('close');
    };
</script>