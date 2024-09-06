<template>
    <v-footer id="app-footer"
              app
              inset
              padless
              height="30"
              class="font-weight-light">
        <v-spacer />
        <AppMessage />
    </v-footer>
</template>

<script setup lang="ts">
   import AppMessage from '@/components/layout/AppMessage.vue';
</script>