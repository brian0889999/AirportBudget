import 'vuetify/styles';
import { createVuetify } from 'vuetify';
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';
import { zhHant } from 'vuetify/locale';

const theme = {
    dark: false,
    colors: {
        primary: '#3f51b5', // 藍色
        'primary-darken-1': '#0D47A1', // 深藍色
        secondary: '#03A9F4', // 亮藍色
        'secondary-darken-1': '#01579B', // 深亮藍色
        accent: '#9C27b0', // 紫色
        info: '#00CAE3', // 青色
        success: '#4CAF50', // 綠色
        warning: '#FB8C00', // 橙色
        error: '#F44336', // 紅色
        background: '#121212', // 深灰色背景
        surface: '#1e1e1e', // 深藍灰色表面
    },
};

const vuetify = createVuetify({
    components,
    directives,
    theme: {
        defaultTheme: 'light',
        themes: {
            theme,
        },
    },
    locale: {
        locale: 'zhHant',
        messages: { zhHant },
    },
})

export default vuetify;