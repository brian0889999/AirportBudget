import axios, { type AxiosRequestConfig } from 'axios';


type DataType = 'data' | 'params';

export interface ApiResponse<T> {
    StatusCode: number;
    Data: T | null;
    Message: string;
}


// 新增下載文件的方法
export const downloadFile = async (url: string, data?: any, dataType?: DataType): Promise<Blob> => {
    const config: AxiosRequestConfig = {
        method: 'get',
        url: url,
        responseType: 'blob', // 指定響應類型為blob
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

    const response = await axios.request(config);
    return response.data;
};

export const postDataAndDownloadFile = async (url: string, data: any): Promise<Blob> => {
    try {
        const config: AxiosRequestConfig = {
            method: 'post',
            url: url,
            data: data,
            responseType: 'blob',
        };

        const response = await axios.request(config);

        if (response.status !== 200) {
            throw new Error(`Unexpected response status: ${response.status}`);
        }

        return response.data;
    } catch (error) {
        console.error('Error during file request:', error);
        throw error;
    }
};