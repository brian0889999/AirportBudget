<template>
    <v-img :src="background"
           style="width: 100%; height: 100vh;"
           cover>
        <v-app-bar color="transparent" flat absolute>
            <!--<v-img :src="logo"
                   class="ml-3"
                   max-height="40"
                   max-width="40"></v-img>-->
            <v-app-bar-title class="font-weight-bold text-primary">
                工務組
                <span class="text-primary">經費預算管理平台</span>
            </v-app-bar-title>

            <v-spacer></v-spacer>

            <!--<v-btn icon>
                <v-icon>mdi-cog</v-icon>
            </v-btn>-->
        </v-app-bar>
        <v-container fill-height fluid>
            <v-row justify="center" align="center" style="width: 100%; height: 100vh;">
                <v-col align="center" cols="12" sm="8" md="6">
                    <v-card class="overlay" max-width="360">
                        <v-card-title class="text-center">
                            <!--<h2>登入</h2>-->
                            <v-img :src="loginImg" width="100%" height="auto" />
                        </v-card-title>
                        <v-card-text>
                            <v-form ref="loginFormRef" @submit.prevent="login" enctype="application/x-www-form-urlencoded">
                                <v-text-field v-model="loginData.Account"
                                              label="帳號"
                                              outlined
                                              :rules="[rules.required]"
                                              clearable>
                                </v-text-field>
                                <v-text-field v-model="loginData.Password"
                                              label="密碼"
                                              outlined
                                              :type="showPassword ? 'text' : 'password' "
                                              @click:append-inner="showPassword = !showPassword"
                                              :rules="[rules.passwordFormat]"
                                              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
                                              clearable>
                                </v-text-field>
                                <v-btn type="submit" color="primary" block :loading="btnLoading">登入</v-btn>
                            </v-form>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </v-img>
</template>


<script setup lang="ts">
import axios from 'axios';
import { ref } from 'vue';
import type { LoginViewModel } from '@/types/apiInterface';
import type { UserViewModel } from '@/types/apiInterface';
import { useRouter } from 'vue-router';
import { post, type ApiResponse } from '../services/api';
import { RULES } from '@/constants/constants';
import background from '@/assets/images/img-login.jpg';
import loginImg from '@/assets/images/經費管理系統.png';

const router = useRouter();

const loginFormRef = ref<HTMLFormElement | null>(null);
const loginData = ref<LoginViewModel>({
    Account: '',
    Password: '',
})
const btnLoading = ref<boolean>(false);
const showPassword = ref<boolean>(false);
const rules = RULES;

    const login = async () => {
        const { valid } = await loginFormRef.value?.validate();
        if (!valid) return;
        btnLoading.value = true;
        const url = '/api/Login';
        const data = loginData.value;
        try {
            const response: ApiResponse<any> = await post<any>(url, data);
            if (response.StatusCode === 200) {
                const jwtToken = response.Data;
                localStorage.setItem('jwtToken', jwtToken);
                router.push({ name: 'main' });
            }

        } catch (error) {
            console.error('登入失敗:', error); // 處理登入失敗的情況
        } finally {
            btnLoading.value = false;
        }
}
</script>

<style scoped>
    .overlay {
        background-color: rgba(255, 255, 255, 0.8); /* 設定背景顏色及透明度 */
    }
</style>