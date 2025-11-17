<template>
  <div class="home-container1">
    <app-navigation></app-navigation>
    <app-footer></app-footer>

  </div>
</template>

<script>
import AppFooter from "@/components/footer.vue";
import AppNavigation from "@/components/navigation.vue";

export default {
  name: 'Home',
  components: {AppNavigation, AppFooter},
  props: {},
  metaInfo: {
    title: 'Sarcastic Affectionate Gerbil',
    meta: [
      {
        property: 'og:title',
        content: 'Sarcastic Affectionate Gerbil',
      },
    ],
  },
}
</script>

<style scoped>
.home-container1 {
  width: 100%;
  display: block;
  min-height: 100vh;
}
 
.home-container2 {
  right: 50px;
  border: 1px solid #ffffff5c;
  bottom: 30px;
  display: flex;
  z-index: 22;
  position: fixed;
  box-shadow: 5px 5px 10px 0px rgba(31, 31, 31, 0.4);
  min-height: auto;
  align-items: center;
  padding-top: 8px;
  padding-left: 12px;
  border-radius: 8px;
  padding-right: 12px;
  padding-bottom: 8px;
  backdrop-filter: blur(6px);
  background-color: rgba(41, 41, 41, 0.41);
}
 
.home-icon1 {
  width: 24px;
  margin-right: 4px;
}
 
.home-text {
  color: white;
  font-size: 13px;
  font-style: normal;
  font-weight: 500;
  line-height: 24px;
}
</style>
