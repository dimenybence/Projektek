package hu.bme.aut.android.todoapp.fragment

import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.widget.ArrayAdapter
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import hu.bme.aut.android.todoapp.R
import hu.bme.aut.android.todoapp.data.TodoItem
import hu.bme.aut.android.todoapp.databinding.DialogNewTodoItemBinding

class NewTodoItemDialogFragment : DialogFragment() {
    interface NewTodoItemDialogListener {
        fun onTodoItemCreated(newItem: TodoItem)
    }

    private lateinit var listener: NewTodoItemDialogListener
    private lateinit var binding: DialogNewTodoItemBinding

    override fun onAttach(context: Context) {
        super.onAttach(context)
        listener = context as? NewTodoItemDialogListener
            ?: throw RuntimeException("Activity must implement the NewTodoItemDialogListener interface!")
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        binding = DialogNewTodoItemBinding.inflate(LayoutInflater.from(context))

        binding.spPriority.adapter = ArrayAdapter(
            requireContext(),
            android.R.layout.simple_spinner_dropdown_item,
            resources.getStringArray(R.array.priorities)
        )

        return AlertDialog.Builder(requireContext())
            .setTitle(R.string.new_todo_item)
            .setView(binding.root)
            .setPositiveButton(R.string.button_ok) { dialogInterface, i ->
                if (isValid()) {
                    listener.onTodoItemCreated(getTodoItem())
                }
                else {
                    binding.etTitle.error = getString(R.string.title_required)
                }
            }
            .setNegativeButton(R.string.button_cancel, null)
            .create()
    }

    companion object {
        const val TAG = "NewTodoItemDialogFragment"
    }

    private fun isValid() = binding.etTitle.text.isNotEmpty()

    private fun getTodoItem() = TodoItem(
        title = binding.etTitle.text.toString(),
        description = binding.etDescription.text.toString(),
        date = binding.etDate.text.toString(),
        time = binding.etTime.text.toString(),
        completed = binding.cbCompleted.isChecked,
        priority = TodoItem.Priority.getByOrdinal(binding.spPriority.selectedItemPosition)
            ?: TodoItem.Priority.LOW
    )
}
