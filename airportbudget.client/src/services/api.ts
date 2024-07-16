import axios, { type AxiosResponse, AxiosError, type AxiosRequestConfig } from 'axios';

type HttpMethod = 'get' | 'post' | 'put' | 'delete';

type DataType = 'data' | 'params';

export interface ApiResponse<T> {
    StatusCode: number;
    Data: T | null;
    Message: string;
}

const handleRequest = async <T>(
    method: HttpMethod,
    url: string,
    data?: any,
    dataType?: DataType,
): Promise<ApiResponse<T>> => {
    try {
        // 定義一個請求配置對象，用於配置 Axios 請求
        const config: AxiosRequestConfig = {
            method: method,
            url: url,
        };

        if (data) {
            switch (dataType) {
                case 'data':
                    config.data = data;
                    break;
                case 'params':
                    config.params = data;
                    break;
            }
        }

        const response: AxiosResponse<ApiResponse<T>> = await axios.request(config);
        return response.data;
    } catch (error: any) {
        if (error.response) {
            // 處理錯誤回應的資料
            const axiosError = error as AxiosError<ApiResponse<any>>;
            const errorResponse = axiosError.response?.data;
            const statusCode = axiosError.response?.status;
            const errorData = errorResponse?.Data;
            const errorMessage = errorResponse?.Message;

            if (errorData) {
                throw new Error(errorData);
            } else if (errorMessage) {
                throw new Error(errorMessage);
            } else {
                switch (statusCode) {
                    case 400:
                        throw new Error("請求錯誤");
                    case 401:
                        throw new Error("未授權");
                    case 403:
                        throw new Error("禁止存取");
                    case 404:
                        throw new Error("找不到資源");
                    case 408:
                        throw new Error("操作逾時");
                    case 500:
                        throw new Error("內部伺服器錯誤");
                    default:
                        throw new Error("未知");
                }
            }
        } else if (error.request) {
            // 處理無回應的情況
            throw new Error('伺服器沒有回應');
        } else {
            // 處理其他錯誤情況
            throw new Error(error.message);
        }
    }
};

export const get = async <T>(url: string, data?: any, dataType?: DataType): Promise<ApiResponse<T>> => {
    return await handleRequest<T>('get', url, data, dataType ?? 'params');
};

export const post = async <T>(url: string, data?: any, dataType?: DataType): Promise<ApiResponse<T>> => {
    return await handleRequest<T>('post', url, data, dataType ?? 'data');
};

export const put = async <T>(url: string, data?: any, dataType?: DataType): Promise<ApiResponse<T>> => {
    return await handleRequest<T>('put', url, data, dataType ?? 'data');
};

export const del = async<T>(url: string, data?: any, dataType?: DataType): Promise<ApiResponse<T>> => {
    return await handleRequest<T>('delete', url, data, dataType ?? 'params');
};
