<template>
    
    <v-container style="width:100%; display:flex;">
        <v-row>
            <v-col cols="12" sm="8" md="6" v-if="!userForm.visible">
                <v-btn text="新增使用者" :loading="loading" v-if="!isEditing" @click="addItem" color="primary" class="mb-4">
                    <template v-slot:load>
                        <v-progress-circular indeterminate></v-progress-circular>
                    </template>
                </v-btn>
                <v-data-table v-if="!isEditing"
                              :loading="loading"
                              :headers="userTableHeaders"
                              :items="paginatedItems"
                              item-key="UserId"
                              items-per-page="12"
                              class="elevation-1"
                              style="width: 100%;"
                              hide-default-footer>
                    <!--這邊items-per-page要跟itemsPerPage一樣-->
                    <template v-slot:top>
                        <v-pagination v-model="page"
                                      :length="pageCount"
                                      @input="updatePage"
                                      class="mb-4"></v-pagination>
                    </template>
                    <template #item.RolePermissionId="{ item }">
                        {{ getRolePermissionLabel(item.RolePermissionId) }}
                    </template>
                    <template #item.System="{ item }">
                        {{ getSystemLabel(item.System) }}
                    </template>
                    <template v-slot:item.Status="{ item }">
                        <v-chip :color="accountStatusColor(item.Status)">
                            {{ accountStatusStr(item.Status) }}
                        </v-chip>
                    </template>
                    <template v-slot:[`item.Edit`]="{ item }">
                        <v-btn @click="editItem(item)" icon class="small-btn">
                            <v-icon>mdi-pencil</v-icon>
                        </v-btn>
                    </template>
                </v-data-table>

            </v-col>
            <v-col cols="12" sm="8" md="6">
                <!--寫死的Table-->
                <AuthTable v-if="!userForm.visible" />
            </v-col>
        </v-row>

        <!-- 表格內容 -->
        <!--<UserForm v-if="isEditing"
              :user="userForm"
              :is-edit-mode="isEditMode"
              @save="saveItem"
              @cancel="cancelEdit" />-->
        <UserForm v-if="userForm.visible"
                  :data="userForm.data"
                  :isEditMode="userForm.isEditMode"
                  @save="saveItem"
                  @cancel="cancelEdit" />
    </v-container>

</template>


<script setup lang="ts">
import axios from 'axios';
import { ref, computed, onMounted } from 'vue';
import type { UserViewModel } from '@/types/apiInterface';
import type { SelectedOption } from '@/types/vueInterface';
import type { VDataTable } from 'vuetify/components';
import { ReverseGroupIdMapping, systemOptionsMapping } from '@/utils/mappings'; // 對應狀態碼到中文
import { get, put, post, type ApiResponse } from '@/services/api';
import { RULES } from '@/constants/constants';
import  AuthTable  from '@/components/modules/AuthTable.vue';
import UserForm from '@/components/modules/UserForm.vue';  

type ReadonlyHeaders = VDataTable['$props']['headers'];

    const loading = ref<boolean>(false);
    const userTableHeaders: ReadonlyHeaders = [
        { title: '姓名', align: 'start', sortable: false, key: 'Name', value: 'Name' },
        { title: '權限', key: 'RolePermissionId', value: 'RolePermissionId' },
        { title: '系統', key: 'System', value: 'System' },
        { title: '狀態', key: 'Status', value: 'Status' },
        { title: '編輯', key: 'Edit', value: 'Edit', sortable: false },
    ];

    const userList = ref<UserViewModel[]>([]);
    const isEditing = ref<boolean>(false);
    const isEditMode = ref<boolean>(true); // 用來區分新增或編輯資料
    const toUTC = (date: Date) => {
        return new Date(date.getTime() - date.getTimezoneOffset() * 60000);
    };
    const defaultUser: UserViewModel = {
        UserId: 0,
        Name: '',
        Account: '',
        Password: '',
        RolePermissionId: 1,
        GroupId: 1,
        Status: true,
        System: undefined,
        LastPasswordChangeDate: toUTC(new Date()),
        ErrCount: 0,
        ErrDate: toUTC(new Date(1990, 0, 1)), // 解決資料庫時差問題
        "Group": {
            "GroupId": 0,
            "GroupName": ""
        },
        "RolePermission": {
            "RolePermissionId": 0,
            "PermissionType": 0
        }
    };
    //const userForm = ref<UserViewModel>({ ...defaultUser }); // 表單的欄位資料
    const userForm = ref({
        visible: false,
        isEditMode: false,
        data: { ...defaultUser }
    });

    // pagination
    const page = ref<number>(1);
    const itemsPerPage = ref<number>(12);
    const pageCount = computed(() => Math.ceil(userList.value.length / itemsPerPage.value));
    const paginatedItems = computed(() => {
        const start = (page.value - 1) * itemsPerPage.value;
        const end = start + itemsPerPage.value;
        return userList.value.slice(start, end);
    });
    const updatePage = (newPage: number) => {
        page.value = newPage;
    };

    const getSystemLabel = (System: number | undefined): string => {
        return System ? systemOptionsMapping[System] : '';
    };
    
    const getRolePermissionLabel = (rolePermissionId: number): string  => {
        const roleMapping: { [key: number]: string } = {
            1: 'A',
            2: 'B',
            3: 'C',
            //4: 'D'
        };
        return roleMapping[rolePermissionId] || rolePermissionId.toString();
    }

    const accountStatusStr = (value: boolean) => {
        switch (value) {
            case true:
                return '啟用';
            case false:
                return '停用';
            default:
                return '';
        }
    };
    const accountStatusColor = (value: boolean) => {
        switch (value) {
            case true:
                return 'green';
            case false:
                return 'red';
            default:
                return '';
        }
    };
    //取得其他使用者
    const fetchUsers = async () => {
        try {
            loading.value = true;
            const url = '/api/User'
            const response: ApiResponse<UserViewModel[]> = await get<UserViewModel[]>(url);
            if (response && response.Data && response.Data) {
                userList.value = response.Data;
            } else {
                console.error("Response data is null or undefined");
            }
        }
        catch (error) {
            console.error(error);
        }
        finally {
            loading.value = false;
        }
    }

    const editItem = (item: UserViewModel) => {
        // 加入編輯邏輯
        //console.log('Edit item:', item);
        //isEditMode.value = true;
        //isEditing.value = true;
        userForm.value.isEditMode = true;
        userForm.value.visible = true;
        userForm.value.data = { ...item };
    };

    const addItem = () => {
        //isEditMode.value = false;
        //isEditing.value = true;
        userForm.value.isEditMode = false;
        userForm.value.visible = true;
        userForm.value.data = { ...defaultUser };
    }

    const saveItem = async () => {
        //isEditing.value = false;
        userForm.value.visible = false;
        await fetchUsers();
        userForm.value.data = { ...defaultUser };
    };

    const cancelEdit = () => {
        //isEditing.value = false;
        userForm.value.visible = false;
        userForm.value.data = { ...defaultUser };
    };
    onMounted(fetchUsers);
</script>

<style scoped>
 .small-btn {
     transform: scale(0.8);
 }

    /*.custom-background .v-input__control {
        background-color: #e0e0e0;*/ /* 灰色背景 */
    /*}

    .custom-background input {
        pointer-events: none;*/ /* 禁止點擊 */
    /*}*/
</style>