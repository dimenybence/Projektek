package hu.bme.aut.android.todoapp.adapter

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.annotation.DrawableRes
import androidx.recyclerview.widget.RecyclerView
import hu.bme.aut.android.todoapp.R
import hu.bme.aut.android.todoapp.data.TodoItem
import hu.bme.aut.android.todoapp.data.TodoItemDatabase
import hu.bme.aut.android.todoapp.databinding.ItemTodoListBinding
import kotlin.concurrent.thread

class TodoAdapter(private val listener: TodoItemClickListener) :
    RecyclerView.Adapter<TodoAdapter.TodoViewHolder>() {

    private val items = mutableListOf<TodoItem>()

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int) = TodoViewHolder(
        ItemTodoListBinding.inflate(LayoutInflater.from(parent.context), parent, false)
    )

    override fun onBindViewHolder(holder: TodoViewHolder, position: Int) {
        val todoItem = items[position]

        holder.binding.ivIcon.setImageResource(getImageResource(todoItem.priority))
        holder.binding.cbDone.isChecked = todoItem.completed
        holder.binding.tvTitle.text = todoItem.title
        holder.binding.tvDescription.text = todoItem.description
        holder.binding.tvDate.text = todoItem.date
        holder.binding.tvTime.text = todoItem.time
        holder.binding.tvPriority.text = todoItem.priority.name

        holder.binding.cbDone.setOnCheckedChangeListener { _, isChecked ->
            todoItem.completed = isChecked
        }

        holder.binding.cbDone.isEnabled = false;

        holder.binding.ibEdit.setOnClickListener {
            listener.onItemEdited(todoItem)
        }
        holder.binding.ibRemove.setOnClickListener {
            listener.onItemChanged(todoItem)
        }
        holder.binding.llItem.setOnClickListener {
            listener.onItemClicked(todoItem)
        }
    }

    @DrawableRes
    private fun getImageResource(priority: TodoItem.Priority): Int {
        return when (priority) {
            TodoItem.Priority.HIGH -> R.drawable.priority_high
            TodoItem.Priority.LOW -> R.drawable.priority_low
        }
    }

    fun addItem(item: TodoItem) {
        items.add(item)
        notifyItemInserted(items.size - 1)
    }

    fun update(todoItems: List<TodoItem>) {
        items.clear()
        items.addAll(todoItems)
        notifyDataSetChanged()
    }

    fun removeItem(item: TodoItem) {
        val position = items.indexOf(item)
        if (position != -1) {
            items.removeAt(position)
            notifyItemRemoved(position)
        }
    }

    override fun getItemCount(): Int = items.size

    interface TodoItemClickListener {
        fun onItemChanged(item: TodoItem)
        fun onItemEdited(item: TodoItem)
        fun onItemClicked(item: TodoItem)
    }

    inner class TodoViewHolder(val binding: ItemTodoListBinding) : RecyclerView.ViewHolder(binding.root)
}
