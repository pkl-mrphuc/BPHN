import { reactive, readonly } from "vue";

const modal = reactive({
  role: []
});

export default function useToggleModal() {
  const toggleModel = () => {
    modal.role.pop();
  };

  const openModal = (role = "") => {
    modal.role.push({ type: role, isOpen: true });
  };

  const hasRole = (role = "") => {
    if (role === "") return false;
    const findRole = modal.role.find(currentRole =>
      currentRole.type === "" ? null : currentRole.type === role
    );
    if (findRole === undefined) return false;
    return findRole.type === role && findRole.isOpen === true ? true : false;
  };

  return {
    state: readonly(modal),
    toggleModel,
    openModal,
    hasRole
  };
}
