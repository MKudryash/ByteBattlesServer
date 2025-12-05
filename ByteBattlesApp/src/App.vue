<template>
  <router-view/>
  <div v-if="$route.query.message" class="global-notification success">
    {{ $route.query.message }}
  </div>
  <div v-if="$route.query.error" class="global-notification error">
    {{ $route.query.error }}
  </div>
</template>