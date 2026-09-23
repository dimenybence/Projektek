package hu.bme.aut.android.todoapp.fragment

import android.R
import android.app.AlertDialog
import android.app.Dialog
import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.widget.ArrayAdapter
import androidx.fragment.app.DialogFragment
import com.github.mikephil.charting.charts.PieChart
import hu.bme.aut.android.todoapp.data.TodoItem
import hu.bme.aut.android.todoapp.databinding.DialogNewTodoItemBinding
import hu.bme.aut.android.todoapp.databinding.DialogViewTodoItemBinding

class ViewTodoItemDialogFragment : DialogFragment() {
    interface ViewTodoItemDialogListener {
        fun getTodoItemDetails(): TodoItem
        fun getAllItems(): List<TodoItem>
    }

    private lateinit var listener: ViewTodoItemDialogListener

    lateinit var binding: DialogViewTodoItemBinding

    override fun onAttach(context: Context) {
        super.onAttach(context)
        listener = context as? ViewTodoItemDialogListener
            ?: throw RuntimeException("Activity must implement the ViewTodoItemDialogListener interface!")
    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        binding = DialogViewTodoItemBinding.inflate(LayoutInflater.from(context))
        binding.etTitle.setText(listener.getTodoItemDetails().title)
        binding.etDescription.setText(listener.getTodoItemDetails().description)
        binding.etDate.setText(listener.getTodoItemDetails().date)
        binding.etTime.setText(listener.getTodoItemDetails().time)
        binding.cbCompleted.isChecked = listener.getTodoItemDetails().completed
        binding.spPriority.adapter = ArrayAdapter(
            requireContext(),
            R.layout.simple_spinner_dropdown_item,
            resources.getStringArray(hu.bme.aut.android.todoapp.R.array.priorities)
        )
        binding.spPriority.setSelection(listener.getTodoItemDetails().priority.ordinal)

        binding.spPriority.isEnabled = false
        binding.cbCompleted.isEnabled = false

        return AlertDialog.Builder(requireContext())
            .setTitle(listener.getTodoItemDetails().title)
            .setView(binding.root)
            .setPositiveButton("Ok") { dialogInterface, i -> }
            .create()
    }

    companion object{
        const val TAG = "ViewTodoItemDialogFragment"
    }
}