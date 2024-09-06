<template>
    <v-card width="100%">
        <v-card-title>
            <v-row>
                <v-col>
                    <h2>{{ formTitle }}</h2>
                </v-col>
            </v-row>
        </v-card-title>
        <v-card-text>
            <v-form ref="userFormRef" @submit.prevent="onSave">
                <v-text-field v-model="localUser.Name" label="姓名" :readonly="isEditMode"
                              :bg-color="nameFieldBgColor"
                              :rules="[rules.required]"></v-text-field>
                <v-select v-model="localUser.RolePermissionId"
                          :items="roleOptions"
                          item-title="text"
                          item-value="value"
                          label="權限"
                          :rules="[rules.required]"></v-select>
                <v-select v-model="localUser.GroupId"
                          :items="ReverseGroupIdMapping" item-title="text" item-value="value"
                          label="組室"
                          :rules="[rules.required]"></v-select>
                <v-select v-model="localUser.System"
                          :items="systemOptionsList" item-title="text" item-value="value"
                          label="系統"></v-select>
                <v-select v-model="localUser.Status"
                          :items="statusOptions" item-title="text" item-value="value"
                          label="狀態*"></v-select>
                <v-text-field v-model="localUser.Account" label="帳號"
                              :rules="[rules.required]"></v-text-field>
                <v-text-field v-model="localUser.Password" label="密碼"
                              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                              :type="showPassword ? 'text' : 'password'"
                              @click:append-inner="showPassword = !showPassword"
                              :rules="[rules.passwordFormat]"></v-text-field>
                <v-btn :text="saveButtonText" type="submit" color="primary" class="mr-2" size="large" :loading="btnLoading">
                    <template #loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
                <v-btn text="取消" @click="onCancel" color="primary" size="large" :loading="btnLoading">
                    <template #loader>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
            </v-form>
        </v-card-text>
    </v-card>
</template>

<script setup lang="ts">
    import { ref, computed, watch, defineProps, defineEmits, type PropType } from 'vue';
    import type { UserViewModel } from '@/types/apiInterface';
    import type { SelectedOption } from '@/types/vueInterface';
    import { RULES } from '@/constants/constants';
    import { ReverseGroupIdMapping, systemOptionsMapping } from '@/utils/mappings';
    import { get, post, put, type ApiResponse } from '@/services/api';
    import { Message } from '@/store/message';

    const props = defineProps({
        data:{ 
            type: Object as PropType<UserViewModel>,
            required: true
        },
        isEditMode: { 
            type: Boolean,
            required: true,
        },
    });

    const emit = defineEmits(['save', 'cancel']);

    const rules = RULES;

    const localUser = ref<UserViewModel>({ ...props.data });
    watch(props.data, (newUser) => {
        localUser.value = { ...newUser };
    });

    const roleOptions = ref<SelectedOption[]>([
        { text: 'A', value: 1 },
        { text: 'B', value: 2 },
        { text: 'C', value: 3 }
    ]);

    const statusOptions = ref<SelectedOption[]>([
        { text: '啟用', value: true },
        { text: '停用', value: false }
    ]);

    const systemOptionsList = ref<SelectedOption[]>([
        { text: '土木', value: 101 },
        { text: '水電', value: 102 },
        { text: '建築', value: 103 },
        { text: '綜合', value: 104 },
        { text: '機械', value: 105 }
    ]);

    const showPassword = ref<boolean>(false);
    const saveButtonText = computed(() => props.isEditMode ? "保存" : "新增");
    const nameFieldBgColor = computed(() => props.isEditMode ? "grey-lighten-1" : "");
    const formTitle = computed(() => props.isEditMode ? '編輯使用者' : '新增使用者');
    const userFormRef = ref<HTMLFormElement | null>(null);
    const btnLoading = ref<boolean>(false);

    const onSave = async () => {
        const { valid } = await userFormRef.value?.validate();
        if (!valid) return;
        try {
            const url = '/api/User'
            const data: UserViewModel = localUser.value;
            btnLoading.value = true;
            let response: ApiResponse<any>;
            if (data.UserId) { // 如果是編輯用put,新增用post
                response = await put<any>(url, data);
                if (response.StatusCode == 200 || response.StatusCode == 201) {
                    Message.success('資料修改成功');
                }
            }
            else {
                response = await post<any>(url, data);
                if (response.StatusCode == 200 || response.StatusCode == 201) {
                    Message.success('資料新增成功');
                }
            };
        } catch (error) {
            console.error(error);
        } finally {
            btnLoading.value = false;
        }
        emit('save');
    };

    const onCancel = () => {
        emit('cancel');
    };
</script>
