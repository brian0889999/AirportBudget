<template>
    <v-dialog v-model="dialogVisible" max-width="500px" persistant>
        <v-card>
            <v-card-title>
                <span class="text-h5">確認操作</span>
            </v-card-title>
            <v-card-text>
                <v-container>
                    <v-row>
                        <v-col cols="12">
                            {{ message }}
                        </v-col>
                    </v-row>
                </v-container>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn text="取消"
                       variant="outlined"
                       @click="closeDialog" />
                <v-btn text="確認"
                       color="primary"
                       variant="elevated"
                       @click="confirmResult" />
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
    import { computed, onMounted, ref, watch } from 'vue';

    const props = defineProps({
        modelValue: {
            type: Boolean,
            required: true,
        },
        title: {
            type: String,
            default: '',
        },
        message: {
            type: String,
            default: '',
        },
    });

    const emit = defineEmits(['update:modelValue', 'result']);

    const dialogVisible = ref<boolean>(false);

    watch(() => props.modelValue, (newVal) => {
        dialogVisible.value = newVal;
    });

    const confirmResult = () => {
        emit('result');
        closeDialog();
    };

    const closeDialog = () => {
        emit('update:modelValue', !dialogVisible.value);
    };
</script>

<style scoped>

</style>