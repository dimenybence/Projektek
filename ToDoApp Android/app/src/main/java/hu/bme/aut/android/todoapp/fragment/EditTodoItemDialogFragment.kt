package hu.bme.aut.android.todoapp.fragment

import android.app.AlertDialog
import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.widget.ArrayAdapter
import androidx.fragment.app.DialogFragment
import hu.bme.aut.android.todoapp.R
import hu.bme.aut.android.todoapp.data.TodoItem
import hu.bme.aut.android.todoapp.databinding.DialogNewTodoItemBinding

class EditTodoItemDialogFragment : DialogFragment() {
    interface EditTodoItemDialogListener {
        fun onTodoItemCreated(item: TodoItem)
        fun getTodoItemDetails(): TodoItem
    }

    private lateinit var listener: EditTodoItemDialogListener
    private lateinit var binding: DialogNewTodoItemBinding
    private lateinit var todoItem: TodoItem

    override fun onAttach(context: Context) {
        super.onAttach(context)
        listener = context as? EditTodoItemDialogListener
            ?: throw RuntimeException("Activity must implement the EditTodoItemDialogListener interface!")
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        binding = DialogNewTodoItemBinding.inflate(LayoutInflater.from(context))

        binding.etTitle.setText(listener.getTodoItemDetails().title)
        binding.etDescription.setText(listener.getTodoItemDetails().description)
        binding.etDate.setText(listener.getTodoItemDetails().date)
        binding.etTime.setText(listener.getTodoItemDetails().time)
        binding.cbCompleted.isChecked = listener.getTodoItemDetails().completed
        binding.spPriority.adapter = ArrayAdapter(
            requireContext(),
            android.R.layout.simple_spinner_dropdown_item,
            resources.getStringArray(R.array.priorities)
        )
        binding.spPriority.setSelection(listener.getTodoItemDetails().priority.ordinal)

        return AlertDialog.Builder(requireContext())
            .setTitle(R.string.edit_todo_item)
            .setView(binding.root)
            .setPositiveButton("Ok") { dialogInterface, i ->
                if (isValid()) {
                    listener.onTodoItemCreated(getTodoItem())
                }
            }
            .create()
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
