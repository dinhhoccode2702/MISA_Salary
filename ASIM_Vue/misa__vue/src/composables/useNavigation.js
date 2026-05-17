import { useRouter } from 'vue-router'

export function useNavigation() {
  const router = useRouter()

  function goToPage(routeName) {
    if (routeName) {
      router.push({ name: routeName })
    } else {
      console.warn("Lỗi: goToPage cần truyền vào tên Route (Name).")
    }
  }
  
  function goToCandidateDetail(id) {
    if (id) {
       router.push({ name: 'CandidateDetail', params: { id } })
    }
  }

  return {
    goToPage,
    goToCandidateDetail
  }
}
