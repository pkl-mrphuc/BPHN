export default function useActionModal() {

  const closeModal = (() => {
    console.log('demo')
  })

  const openModal = ((dialogComponent) => {
    console.log(dialogComponent)
  })

  return {
    openModal,
    closeModal
  }

}