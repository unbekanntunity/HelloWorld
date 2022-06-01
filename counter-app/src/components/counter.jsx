import React, { Component } from 'react';

class Counter extends Component {
    //constructor() {
    //    super();
    //    this.handleIncrement = this.handleIncrement.bind(this);
    //}

    componentDidUpdate(prevprops, prevState) {
        console.log('preProps', prevprops);
        console.log('preState', prevState);
        if (prevprops.counter.value !== this.props.counter.value)
        {
            //Ajax call
        }
    }

    renderTags() {
        if (this.state.tags.length === 0) {
            return <p>There are no tags!</p>
        }
        else {
            return <ul>{this.state.tags.map(tag => <li key={tag}>{tag}</li>)}</ul>
        }
    }
    render() {
        return (
            <div>
                <h4>{this.props.children}</h4>

                {/*this.props.tags.length === 0 && 'Please create a new tag!'*/}

                <span className={this.getBadgeClasses()}>{this.formatCount()}</span>
                <button onClick={() => this.props.onIncremeant(this.props.counter)} className="btn btn-secondary btn-sm">Increment</button>
                <button onClick={() => this.props.onDelete(this.props.counter.id)} className="btn btn-danger btn-sm m-2">Delete</button>
                {/*this.renderTags()*/}
            </div>
        );
    }

    getBadgeClasses() {
        let classes = "badge m-2 ";
        classes += (this.props.counter.value === 0) ? "bg-warning text-dark" : "bg-primary"

        return classes;
    }

    formatCount() {
        const { value } = this.props.counter;
        return value === 0 ? 'Zero' : value;
    }
}

export default Counter;