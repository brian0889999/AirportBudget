declare module 'vue3-cookies' {
    interface CookieOptions {
        path?: string;
        domain?: string;
        secure?: boolean;
        'max-age'?: number;
        expires?: string | Date;
        'same-site'?: 'Strict' | 'Lax' | 'None';
    }

    interface Cookies {
        set(name: string, value: any, options?: CookieOptions | string): void;
        get(name: string): any;
        remove(name: string, options?: CookieOptions): void;
    }

    export function useCookies(): { cookies: Cookies };
}
