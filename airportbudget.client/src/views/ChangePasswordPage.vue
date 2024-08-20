<template>
    <v-img :src="background"
           style="width: 100%; height: 100vh;"
           cover>
        <v-container fill-height fluid>
            <v-row justify="center" align="center" style="width: 100%; height: 100vh;">
                <v-col align="center" cols="12" sm="8" md="6">
                    <v-card class="overlay" max-width="360">
                        <v-card-title class="text-center">
                            <h2>更改密碼</h2>
                        </v-card-title>
                        <v-card-text>
                            <v-form ref="changePasswordFormRef" @submit.prevent="changePassword" enctype="application/x-www-form-urlencoded">
                                <v-text-field v-model="passwordData.OldPassword"
                                              label="舊密碼"
                                              outlined
                                              :type="showOldPassword ? 'text' : 'password' "
                                              @click:append-inner="showOldPassword = !showOldPassword"
                                              :rules="[rules.required]"
                                              :append-inner-icon="showOldPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                              clearable>
                                </v-text-field>
                                <v-text-field v-model="passwordData.NewPassword"
                                              label="新密碼"
                                              outlined
                                              :type="showNewPassword ? 'text' : 'password' "
                                              @click:append-inner="showNewPassword = !showNewPassword"
                                              :rules="[rules.passwordFormat]"
                                              :append-inner-icon="showNewPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                              clearable>
                                </v-text-field>
                                <v-text-field v-model="passwordData.ConfirmPassword"
                                              label="確認新密碼"
                                              outlined
                                              :type="showConfirmPassword ? 'text' : 'password' "
                                              @click:append-inner="showConfirmPassword = !showConfirmPassword"
                                              :rules="[rules.confirmPassword]"
                                              :append-inner-icon="showConfirmPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                              clearable>
                                </v-text-field>
                                <v-btn type="submit" color="primary" block :loading="btnLoading">確定更改</v-btn>
                            </v-form>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-img>
</template>

<script setup lang="ts">
    import { ref } from 'vue';
    import { useRouter, useRoute } from 'vue-router';
    import { post, put, type ApiResponse } from '@/services/api';
    import { RULES } from '@/constants/constants';
    import background from '@/assets/images/songshan2.webp';

    const router = useRouter();
    const route = useRoute();
    const changePasswordFormRef = ref<HTMLFormElement | null>(null);
    const passwordData = ref({
        OldPassword: '',
        NewPassword: '',
        ConfirmPassword: ''
    });
    const btnLoading = ref<boolean>(false);
    const showOldPassword = ref<boolean>(false);
    const showNewPassword = ref<boolean>(false);
    const showConfirmPassword = ref<boolean>(false);
    const rules = {
        ...RULES,
        confirmPassword: (value: string) => value === passwordData.value.NewPassword || '密碼不一致',
    };
    const userId = ref(route.query.userId);
    //console.log(userId.value);
    const changePassword = async () => {
        const { valid } = await changePasswordFormRef.value?.validate();
        if (!valid) return;

        btnLoading.value = true;
        const url = '/api/Login/ChangePassword';
        const data = { ...passwordData.value, UserId: userId.value };

        try {
            //console.log(data);
            const response: ApiResponse<any> = await put<any>(url, data);
            if (response.StatusCode === 200) {
                alert('密碼更改成功');
                router.push({ name: 'login' }); // 跳轉到登入頁面
            } else {
                alert(response.Data); // 顯示錯誤訊息
            }
        } catch (error: any) {
            console.error('更改密碼失敗:', error);
            alert('更改密碼失敗');
        } finally {
            btnLoading.value = false;
        }
    };
</script>

<style scoped>
    .overlay {
        background-color: rgba(255, 255, 255, 0.8);
    }
</style>
