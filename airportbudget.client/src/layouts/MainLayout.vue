<template>
    <div style="display: contents">
        <AppBar />
        <AppDrawer />
        <AppView />
        <AppFooter />
    </div>
</template>

<script setup lang="ts">
    import AppBar from '@/components/layout/AppBar.vue';
    import AppDrawer from '@/components/layout/AppDrawer.vue';
    import AppView from '@/components/layout/AppView.vue';
    import AppFooter from '@/components/layout/AppFooter.vue';
</script>

<style scoped>

</style>