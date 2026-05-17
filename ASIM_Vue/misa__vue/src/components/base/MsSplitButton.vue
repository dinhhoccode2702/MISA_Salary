<template>
  <div ref="rootRef" class="ms-split-button flex" :class="{ 'is-open': isOpen }">
    <MsButton
      :type="type"
      class="btn-left flex-1"
      :icon-class="iconClass"
      @click="$emit('click')"
    >
      <slot></slot>
    </MsButton>
    <div 
      class="btn-right flex align-center justify-center"
      :class="type"
      @click="handleArrowClick"
    >
      <span class="ms-icon-base ms-icon--chevron-down" :style="{ backgroundColor: arrowIconColor }"></span>
    </div>

    <!-- Dropdown menu (optional) -->
    <div v-if="hasMenu" v-show="isOpen" class="split-menu">
      <div
        v-for="item in menuItems"
        :key="item.key"
        class="split-menu-item flex align-center"
        @click="selectItem(item)"
      >
        <span v-if="item.iconClass" class="ms-icon-base split-menu-icon" :class="item.iconClass"></span>
        <span class="split-menu-label">{{ item.label }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onBeforeUnmount, onMounted, ref } from 'vue';
import MsButton from './MsButton.vue';

const props = defineProps({
  type: {
    type: String,
    default: 'primary',
  },
  iconClass: {
    type: String,
    default: '',
  },
  /** Optional dropdown items: [{ key, label, iconClass? }] */
  menuItems: {
    type: Array,
    default: null,
  },
});

const emit = defineEmits(['click', 'arrow-click', 'select']);

const rootRef = ref(null);
const isOpen = ref(false);
const hasMenu = computed(() => Array.isArray(props.menuItems) && props.menuItems.length > 0);

const closeMenu = () => {
  isOpen.value = false;
};

const handleArrowClick = () => {
  if (!hasMenu.value) {
    emit('arrow-click');
    return;
  }
  isOpen.value = !isOpen.value;
  emit('arrow-click');
};

const selectItem = (item) => {
  emit('select', item?.key);
  closeMenu();
};

const onDocMouseDown = (e) => {
  if (!isOpen.value) return;
  const el = rootRef.value;
  if (!el) return;
  if (el.contains(e.target)) return;
  closeMenu();
};

onMounted(() => {
  document.addEventListener('mousedown', onDocMouseDown);
});

onBeforeUnmount(() => {
  document.removeEventListener('mousedown', onDocMouseDown);
});

const arrowIconColor = computed(() => {
  if (props.type === 'primary') return 'white';
  return 'var(--text-primary)';
});
</script>

<style scoped>
.ms-split-button {
  border-radius: 4px;
  overflow: visible;
  position: relative;
}

.btn-left {
  border-top-right-radius: 0 !important;
  border-bottom-right-radius: 0 !important;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
  min-width: auto;
}

.btn-right {
  width: 32px;
  cursor: pointer;
  border-top-right-radius: 4px;
  border-bottom-right-radius: 4px;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-right.primary {
  background-color: var(--color-primary);
}

.btn-right.primary:hover {
  background-color: var(--color-primary-hover);
}

.btn-right.outline {
  background-color: transparent;
  border: 1px solid var(--border-color);
  border-left: none;
}

.btn-right.outline:hover {
  background-color: #f5f5f5;
}

.ms-icon--chevron-down {
  width: 16px;
  height: 16px;
}

.split-menu {
  position: absolute;
  top: calc(100% + 6px);
  right: 0;
  width: 260px;
  background: #fff;
  border: 1px solid var(--border-color, #e0e0e0);
  border-radius: 4px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  z-index: 50;
  padding: 6px 0;
}

.split-menu-item {
  padding: 10px 12px;
  cursor: pointer;
  user-select: none;
}

.split-menu-item:hover {
  background: #f5f5f5;
}

.split-menu-icon {
  width: 16px;
  height: 16px;
  margin-right: 8px;
}

.split-menu-label {
  font-size: 13px;
  color: var(--text-primary);
}
</style>
