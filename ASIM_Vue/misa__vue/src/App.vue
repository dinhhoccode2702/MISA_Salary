<script setup>
import { ref } from 'vue'
import TheHeader from './components/layout/TheHeader.vue'
import TheSidebar from './components/layout/TheSidebar.vue'
import MsButton from './components/base/MsButton.vue'
import MsModal from './components/common/MsModal.vue'
import MsToast from './components/common/MsToast.vue'
import { useToast } from './composables/useToast'

const { toasts, confirms, closeConfirm } = useToast()
const isSidebarCollapsed = ref(false)
</script>

<template>
  <div class="app-container">
    <TheHeader />
    <div class="main-layout flex">
      <TheSidebar @collapse-change="isSidebarCollapsed = $event" />
      <main
        class="main-content"
        :class="{ 'main-content--sidebar-collapsed': isSidebarCollapsed }"
      >
        <router-view></router-view>
      </main>
    </div>

    <!-- Toast Container -->
    <div class="toast-container">
      <MsToast 
        v-for="toast in toasts" 
        :key="toast.id" 
        v-bind="toast"
      />
    </div>

    <MsModal
      v-for="confirm in confirms"
      :key="confirm.id"
      show
      :title="confirm.title"
      :width="confirm.width"
      @close="closeConfirm(confirm.id, false)"
    >
      <div class="confirm-message">{{ confirm.message }}</div>
      <template #footer>
        <MsButton type="outline" class="m-r-8" @click="closeConfirm(confirm.id, false)">
          {{ confirm.cancelLabel }}
        </MsButton>
        <MsButton
          type="primary"
          :bgColor="confirm.okVariant === 'danger' ? 'var(--color-error)' : ''"
          @click="closeConfirm(confirm.id, true)"
        >
          {{ confirm.okLabel }}
        </MsButton>
      </template>
    </MsModal>
  </div>
</template>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100%;
  min-width: 0;
  overflow: hidden;
}

.main-layout {
  flex: 1;
  margin-top: var(--header-height);
  width: 100%;
  min-width: 0;
}

.main-content {
  flex: 1;
  margin-left: var(--sidebar-width);
  min-width: 0;
  background-color: var(--app-bg);
  height: calc(100vh - var(--header-height));
  display: flex;
  flex-direction: column;
  overflow: hidden; /* Prevent body scroll, use internal scroll */
  transition: margin-left 0.3s ease;
}

.main-content--sidebar-collapsed {
  margin-left: var(--sidebar-collapsed-width);
}

.confirm-message {
  color: var(--text-primary);
  font-size: 14px;
  line-height: 1.5;
  white-space: pre-line;
}

.toast-container {
  position: fixed;
  top: 8px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 12px;
  align-items: center;
}
</style>
