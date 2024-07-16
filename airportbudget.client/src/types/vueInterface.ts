export interface Crumb {
    title: string;
    icon?: string;
    href: string;
    childs?: Crumb[];
}

export interface Breadcrumb {
    title?: string;
    disabled: boolean;
    href: string;
}

export interface RouteItemMeta {
    title?: string;
    icon?: string;
    disabled?: string;
}

export interface SelectedOption {
    text: string;
    value: any;
}