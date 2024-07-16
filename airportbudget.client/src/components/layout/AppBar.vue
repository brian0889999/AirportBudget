<template>
    <v-app-bar color="primary"
               prominent
               elevation="0">
        <v-app-bar-nav-icon variant="text" @click.stop="toggleDrawer(!drawer)"></v-app-bar-nav-icon>
        <v-spacer />
        <v-spacer />
        <v-spacer />
        <v-spacer />
        <v-spacer />
        <v-spacer />
        <v-toolbar-title>經費管理/歡迎&nbsp;&nbsp;{{ user.Name }}</v-toolbar-title>
        <v-btn @click="mainRoute">首頁</v-btn>
        <v-btn @click="logout">登出</v-btn>

    </v-app-bar>
</template>

<script setup lang="ts">
    import { computed, onMounted, ref } from 'vue';
    import { useStore } from '@/store/index';
    import { useRouter } from 'vue-router';
    import { get, type ApiResponse } from '@/services/api';
    import type { UserViewModel } from '@/types/apiInterface';
    const router = useRouter();
    const defaultUser: UserViewModel = {
        UserId: 0,
        Name: '',
        Account: '',
        Password: '',
        RolePermissionId: 0,
        GroupId: 0,
        Status: false,
        System: '',
        LastPasswordChangeDate: undefined,
        ErrCount: 0,
        ErrDate: new Date(1990, 0, 1)
};
    const user = ref<UserViewModel>(defaultUser); 
    onMounted(async () => {
        await getCurrentUser();
    });

    const getCurrentUser = async () => {
        const url = '/api/User/Current';
        try {
            const response: ApiResponse<UserViewModel> = await get<UserViewModel>(url);
            if (response.StatusCode === 200) {
                const data = response.Data;
                user.value = data ? data : defaultUser;
                console.log(user.value);
            }
            else {
                console.error(response.Data ?? response.Message);
            }
        }
        catch (error: any) {
            console.error(error.message);
        }
    };

    const logout = () => {
        if (confirm("確定要登出嗎?")) {
            localStorage.removeItem('jwtToken');
            router.push('/login');
        } else {
            return false;
        }

    }
    const store = useStore();

    const drawer = computed(() => {
        return store.drawer;
    })

    const rail = computed(() => {
        return store.rail;
    })
    const toggleDrawer = (value?: boolean) => {
        store.setDrawer(value != undefined ? value : !store.drawer);
    }

    const toggleRail = (value?: boolean) => {
        store.setRail(value != undefined ? value : !store.rail);
    }

    const mainRoute = () => {
        router.push('/main/PublicWorksGroup');
    }
</script>

<style scoped>

</style>